//#define MY_DEBUG

using BulkMessagesWebServer.DBObjects;
using ChatMessengersService.FillSiMedDB;
using SiMed.ChatMessengers.Umnico;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using BulkMessagesWebServer;
using BulkMessagesWebServer.DataModel;
using ChatMessengersService.MassMessagesUmnicoSenders;
using ChatAggregatorMessage = SiMed.Clinic.DataModel.Message;

namespace ChatMessengersService
{
    public partial class MainService : ServiceBase
    {
        private MassMessagesLogsWorker m_MassMessagesLogger;
        private IMessengerCommon m_ChatAggregator;
        private IMassMessagesRepository m_MassMessagesRepository;
        private WatsonBulkMessagesWebServer m_BulkMessagesWebServer;
        private MassMessagesUmnicoSenderService m_MassMessagesUmnicoSenderService;
        private readonly ChatMessengers m_ChatMessengers;
        private Exception m_LastException;
        //-----------------------------------------------------
        public Exception LastException
        {
            get => m_LastException;
        }
        //-----------------------------------------------------
        public MainService(string _TestLaunchNameConnectionString = null)
        {
            InitializeComponent();

            m_MassMessagesLogger = new MassMessagesLogsWorker(Application.StartupPath);

            //=====================================================================================================
            #region Инициализация чат-аггрегатора для приёма сообщений
            //=====================================================================================================
            string _GlobalOptionsPath = Application.StartupPath + "\\Globaloptions.txt";
            string _LocalOptionsPath = Application.StartupPath + "\\Localoptions.txt";

            string _ChatAggregatorType = ConfigurationManager.AppSettings["ChatAggregatorType"];

            Action<string> _LogDelegate = (string message) =>
            {
                DoAction(() =>
                {
                    throw new Exception(message);
                });
            };

            DoAction(() =>
            {
                m_ChatAggregator = MotherObject.CreateChatAggregator(_ChatAggregatorType, _GlobalOptionsPath, _LogDelegate, _LocalOptionsPath);
            },
            "Ошибка в конструкторе MainService");

            m_ChatMessengers = new ChatMessengers(m_ChatAggregator, DoAction);
            //=====================================================================================================
            #endregion
            //=====================================================================================================

            //=====================================================================================================
            #region Инициализация репозитория для работы с БД
            //=====================================================================================================
            string _MassMessagesRepositoryType = ConfigurationManager.AppSettings["MassMessagesRepositoryType"];

            string _NameConnectionString;

            if (_TestLaunchNameConnectionString != null)
            {
                _NameConnectionString = _TestLaunchNameConnectionString;
            }
            else
            {
                _NameConnectionString = ConfigurationManager
                    .ConnectionStrings["ChatMessengersService.Properties.Settings.dtsChatMessenger"].Name;
            }

            DoAction(() =>
            {
                m_MassMessagesRepository = MotherObject
                    .CreateMassMessagesRepository(_MassMessagesRepositoryType, _NameConnectionString);
            },
            "Ошибка в конструкторе MainService");
            //=====================================================================================================
            #endregion
            //=====================================================================================================

            //=====================================================================================================
            #region Инициализация сервера массовой рассылки
            //=====================================================================================================
            DoAction(() =>
            {
                m_BulkMessagesWebServer = MotherObject.CreateWatsonBulkMessagesWebServer(m_MassMessagesRepository);
            },
            "Ошибка в конструкторе MainService");
            //=====================================================================================================
            #endregion
            //=====================================================================================================

            //=====================================================================================================
            #region Инициализация сервиса для отправки сообщений массовой рассылки
            //=====================================================================================================
            DoAction(() =>
            {
                m_MassMessagesUmnicoSenderService = MotherObject
                    .CreateMassMessagesUmnicoSenderService(m_ChatAggregator, m_MassMessagesRepository);
            },
            "Ошибка в конструкторе MainService");
            //=====================================================================================================
            #endregion
            //=====================================================================================================

            CanStop = true; // службу можно остановить
            CanPauseAndContinue = true; // службу можно приостановить и затем продолжить
            AutoLog = true; // служба может вести запись в лог
        }
        //-----------------------------------------------------
        protected override void OnStart(string[] args)
        {
            Thread _ChatMessengerServiceThread = new Thread(new ThreadStart(m_ChatMessengers.Start));
            _ChatMessengerServiceThread.Start();

            Thread _BulkMassMessagesWebServerThread = new Thread(new ThreadStart(m_BulkMessagesWebServer.Start));
            _BulkMassMessagesWebServerThread.Start();

            Thread _SenderServiceThread = new Thread(new ThreadStart(m_MassMessagesUmnicoSenderService.Start));
            _SenderServiceThread.Start();
        }
        //-----------------------------------------------------
        protected override void OnStop()
        {
            m_ChatMessengers.Stop();
            m_BulkMessagesWebServer.Stop();
            m_MassMessagesUmnicoSenderService.Stop();
            Thread.Sleep(1500);
        }
        //-----------------------------------------------------
        private bool DoAction(Action action, string message = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    ex.Data.Add("ExceptionInfo", message);
                    m_ChatAggregator?.SaveExceptionToLog(ex, message);
                }
                m_ChatAggregator?.SaveExceptionToLog(ex, "");

                LogData _LogData = new LogData().SetExceptionData(ex);

                if (message != null)
                {
                    _LogData.SetDeveloperComment(message);
                }

                m_MassMessagesLogger.SaveMessage(_LogData);

                m_LastException = ex;
#if MY_DEBUG
                throw;
#endif
                return false;
            }
            return true;
        }
        //-----------------------------------------------------
    }
    //-----------------------------------------------------
    class ChatMessengers
    {
        private readonly IMessengerCommon m_ChatAggregator;
        private readonly Func<Action, string, bool> m_DoAction;
        private bool m_IsWorkCycle;
        //-----------------------------------------------------
        public ChatMessengers(IMessengerCommon _ChatAggregator, Func<Action, string, bool> _DoAction)
        {
            m_ChatAggregator = _ChatAggregator;
            m_DoAction = _DoAction;
        }
        //-----------------------------------------------------
        public void Start()
        {
#if MY_DEBUG
            Thread.Sleep(20000);
#endif
            m_DoAction.Invoke(() =>
            {
                m_IsWorkCycle = true;

                while (m_IsWorkCycle)
                {
                    IEnumerable<ChatAggregatorMessage> _MessagesReceivedFromChatAggregator = m_ChatAggregator.CheckNewMessages();

                    bool _HasNewMessages = _MessagesReceivedFromChatAggregator != null &&
                        _MessagesReceivedFromChatAggregator.Count() > 0;

                    if (_HasNewMessages)
                    {
                        DBLogger _DBLogger = new DBLogger(m_DoAction);
                        _DBLogger.Load();

                        IEnumerable<string> _BlockingMessengerDialogDataView = _DBLogger.GetMessengerDialogUIdsForBlockingMessengerDialogs();

                        IEnumerable<ChatAggregatorMessage> _FilteredMessages =
                            _MessagesReceivedFromChatAggregator.FilterOutWantedMessages(_BlockingMessengerDialogDataView);

                        if (_FilteredMessages.Count() > 0)
                        {
                            MainFillerSiMedDB fillerSiMedDBFacade = new MainFillerSiMedDB(m_DoAction, _DBLogger, m_ChatAggregator);

                            foreach (ChatAggregatorMessage _Message in _FilteredMessages)
                            {
                                m_DoAction.Invoke(() =>
                                {
                                    fillerSiMedDBFacade.Message = _Message;
                                    fillerSiMedDBFacade.FillDB();
                                },
                                $"Ошибка в сообщении с Id = {_Message.Id}");
                            }
                        }
                    }

                    Thread.Sleep(7000);
                }                
            },
            "Произошло падение общего обработчика ошибок в потоке для работы с сообщениями из чат-аггрегатора!");
        }
        //-----------------------------------------------------
        public void Stop()
        {
            m_IsWorkCycle = false;
        }
        //-----------------------------------------------------
    }
    //-----------------------------------------------------
}
