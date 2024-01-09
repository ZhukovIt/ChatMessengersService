//#define MY_DEBUG

using BulkMessagesWebServer.DataModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebserver;
using System.Net;
using BulkMessagesWebServer.DBObjects;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("BulkMessagesWebServerTests")]

namespace BulkMessagesWebServer
{
    public sealed class WatsonBulkMessagesWebServer
    {
        private MainOptions m_Options;
        private readonly MassMessagesLogsWorker m_MassMessagesLogsWorker;
        private readonly MassMessagesPutter m_MassMessagesPutter;
        private Exception m_LastException;
        private Server m_Server;
        private Mutex m_Mutex;
        private int m_MutexWaitMS;
        private bool m_ServerIsLaunch;
        //---------------------------------------------------------------------------
        public bool ServerIsLaunch
        {
            get => m_ServerIsLaunch;
            private set => m_ServerIsLaunch = value;
        }
        //---------------------------------------------------------------------------
        internal Server Server
        {
            get
            {
                return m_Server;
            }
        }
        //---------------------------------------------------------------------------
        public Exception LastException
        {
            get
            {
                return m_LastException;
            }

            private set
            {
                m_LastException = value;
            }
        }
        //---------------------------------------------------------------------------
        public WatsonBulkMessagesWebServer(IMassMessagesRepository _Repository, MainOptions _Options, string _BranchName)
        {
            m_MassMessagesLogsWorker = new MassMessagesLogsWorker(Application.StartupPath);

            if (_Repository == null)
            {
                throw new ArgumentNullException("Объект типа IMassMessagesRepository оказался равен null!");
            }

            m_MassMessagesPutter = new MassMessagesPutter(_Repository, m_MassMessagesLogsWorker)
            {
                BranchName = _BranchName
            };
            m_Options = _Options;
            m_Mutex = new Mutex();
            m_MutexWaitMS = 10000;
            m_ServerIsLaunch = false;
        }
        //---------------------------------------------------------------------------
        public void Start()
        {
#if MY_DEBUG
            Thread.Sleep(20000);
#endif
            try
            {
                if (m_Options == null)
                    throw new Exception("Не заданы настройки модуля");

                m_Server = new Server(m_Options.Domain, m_Options.Port, false, DefaultRoute);
                m_Server.Settings.IO.MaxRequests = 10;
                m_Server.Routes.Static.Add(HttpMethod.GET, "/api/UmnicoBulkMessagesWebServer/VerifyGuid", VerifyGuid);
                m_Server.Routes.Static.Add(HttpMethod.POST, "/api/UmnicoBulkMessagesWebServer/SendMessages", SendMessages);
                m_Server.Start();

                m_MassMessagesLogsWorker.SaveMessage(new LogData()
                {
                    Source = GetType().FullName,
                    Message = "Сервер сохранения сообщений массовой рассылки успешно запущен!"
                });

                m_ServerIsLaunch = true;
            }
            catch (Exception ex)
            {
                LogData _LogData = new LogData().SetExceptionData(ex);

                if (m_Options != null)
                {
                    _LogData = _LogData.SetDeveloperComment(
                        $"Не удалось запустить прослушивание порта №{m_Options.Port} по адресу http://{m_Options.Domain}");
                }

                m_MassMessagesLogsWorker.SaveMessage(_LogData);

                m_LastException = ex;
                m_Server?.Dispose();
                m_Server = null;
                m_ServerIsLaunch = false;
            }
        }
        //---------------------------------------------------------------------------
        public void Stop()
        {
            try
            {
                m_MassMessagesLogsWorker.SaveMessage(new LogData()
                {
                    Source = GetType().FullName,
                    Message = "Сервер сохранения сообщений массовой рассылки остановлен!"
                });
                m_ServerIsLaunch = false;
                m_Server?.Stop();
            }
            finally
            {
                m_Server?.Dispose();
                m_Server = null;
            }
        }
        //---------------------------------------------------------------------------
        public async Task VerifyGuid(HttpContext ctx)
        {
            string _ErrorMessage;
            SendMessagesResponse _Response = new SendMessagesResponse();

            _Response.Success = VerifyGuid(ctx.Request.Headers["Authorization"], out _ErrorMessage);

            if (_Response.Success)
            {
                ctx.Response.StatusCode = 200;
            }
            else
            {
                ctx.Response.StatusCode = 401;
                _Response.InfoMessage = _ErrorMessage;
            }

            await ctx.Response.Send(JsonConvert.SerializeObject(_Response, Formatting.Indented));
        }
        //---------------------------------------------------------------------------
        private bool VerifyGuid(string _Authorization, out string _Error)
        {
            _Error = null;

            try
            {
                if (_Authorization == null)
                {
                    throw new Exception("Данные для авторизации отсутствуют!");
                }

                string[] _AuthData = _Authorization.Split();

                if (_AuthData.Length < 2 || _AuthData[0] != "GUID")
                {
                    throw new Exception("GUID для авторизации отсутствует!");
                }

                if (_AuthData[1] != m_Options.GUID)
                {
                    throw new Exception("Авторизация не пройдена, так как GUID неправильный!");
                }
            }
            catch (Exception ex)
            {
                m_MassMessagesLogsWorker.SaveMessage(new LogData()
                    .SetExceptionData(ex)
                    .SetDeveloperComment($"Authorization = \"{_Authorization}\""));
                m_LastException = ex;
                _Error = ex.Message;
                return false;
            }

            return true;
        }
        //---------------------------------------------------------------------------
        private async Task DefaultRoute(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.Send("СиМед - Сервер получения сообщений массовой рассылки для службы чат-мессенджеров. На связи!");
        }
        //---------------------------------------------------------------------------
        private async Task SendMessages(HttpContext ctx)
        {
            SendMessagesResponse _Response;

            try
            {
                string _CommandName = "SendMessages";
                string _ErrorMessage;

                if (!VerifyGuid(ctx.Request.Headers["Authorization"], out _ErrorMessage))
                {
                    _Response = new SendMessagesResponse()
                    {
                        Success = false,
                        InfoMessage = _ErrorMessage
                    };
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Send(JsonConvert.SerializeObject(_Response, Formatting.Indented));
                    return;
                }

                SendMessagesRequest _Request = ctx.Request.DataAsJsonObject<SendMessagesRequest>();

                m_MassMessagesLogsWorker.SaveMessage(
                    new LogData()
                    {
                        Source = GetType().Name,
                        Message = $"Поступила команда {_CommandName} с IP = {ctx.Request.Source.IpAddress}"
                    });

                bool _MutexRes = m_Mutex.WaitOne(m_MutexWaitMS);

                try
                {
                    if (!_MutexRes)
                    {
                        _Response = new SendMessagesResponse()
                        {
                            Success = false,
                            InfoMessage = $"Не удалось получить монопольный доступ к сервису отправки сообщений за {m_MutexWaitMS} мс"
                        };
                        ctx.Response.StatusCode = 500;
                        await ctx.Response.Send(JsonConvert.SerializeObject(_Response, Formatting.Indented));
                        throw new InvalidOperationException(_Response.InfoMessage);
                    }

                    string _IpAddress = ctx.Request.Source.IpAddress;

                    if (_Request.PersonsInfo.Count() == 0)
                    {
                        throw new InvalidOperationException($"Пришла пустая команда c Ip = {_IpAddress}");
                    }

                    _Response = m_MassMessagesPutter.PutMessagesInDataBase(_Request);
                }
                finally
                {
                    if (_MutexRes)
                    {
                        m_Mutex.ReleaseMutex();
                    }
                }

                ctx.Response.StatusCode = 200;
                await ctx.Response.Send(JsonConvert.SerializeObject(_Response, Formatting.Indented));
            }
            catch (Exception ex)
            {
                m_MassMessagesLogsWorker.SaveMessage(new LogData().SetExceptionData(ex));
                m_LastException = ex;
                _Response = new SendMessagesResponse()
                {
                    Success = false,
                    InfoMessage = CreateMessagesWithInnerException(ex),
                    MesUmnGuids = null
                };
                ctx.Response.StatusCode = 500;
                await ctx.Response.Send(JsonConvert.SerializeObject(_Response, Formatting.Indented));
            }
        }
        //---------------------------------------------------------------------------
        private string CreateMessagesWithInnerException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return ex.Message + "\r\n\r\n" + CreateMessagesWithInnerException(ex.InnerException);
        }
        //---------------------------------------------------------------------------
    }
}
