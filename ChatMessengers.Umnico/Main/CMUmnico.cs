using SiMed.ChatMessengers.Umnico.GUI;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;


namespace SiMed.ChatMessengers.Umnico
{
    public class CMUmnico : IMessengerCommon
    {
        /// <summary>
        /// Данные для авторизации существуют
        /// </summary>
        private bool m_CheckAuthorization;
        /// <summary>
        /// Авторизация под каким-либо сотрудником была проведена
        /// </summary>
        private bool m_CheckLogin;

        private AuthorizationInfo m_AuthInfo;
        private Manager m_Manager;
        private  CMUmnicoGlobalOptions m_GlobalOptions;
        private CMUmnicoLocalOptions m_LocalOptions;
        private readonly Action<string> m_LogDelegate;
        private Exception m_LastException;
        public const string ModuleType = "Umnico";
        public static Clinic.DataModel.ClinicChatAggregatorApplicationType m_appType;
        //-------------------------------------------------------------------------------------
        public Exception LastException
        {
            get { return m_LastException; }
        }
        //-------------------------------------------------------------------------------------
        CMUmnicoGlobalOptions globalOptions
        {
            get
            {
                if (m_GlobalOptions == null)
                {
                    m_GlobalOptions = new CMUmnicoGlobalOptions();
                }

                return m_GlobalOptions;
            }

            set
            {
                m_GlobalOptions = value;
            }
        }
        //-------------------------------------------------------------------------------------
        CMUmnicoLocalOptions localOptions
        {
            get
            {
                if (m_LocalOptions == null)
                    m_LocalOptions = new CMUmnicoLocalOptions();
                return m_LocalOptions;
            }

            set
            {
                m_LocalOptions = value;
            }
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_LogDelegate"> Передать функцию логирования/вывода на экран </param>
        public CMUmnico(Action<string> _LogDelegate)
        {
            m_CheckAuthorization = false;
            m_CheckLogin = false;
            m_LogDelegate = _LogDelegate;
            ServicePointManager.Expect100Continue = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls |
                (SecurityProtocolType)768 |              
                (SecurityProtocolType)3072;
        }
        //-------------------------------------------------------------------------------------
        #region Реализация методов интерфейса IMessengerCommon
        //-------------------------------------------------------------------------------------
        public bool CheckConnection()
        {
            return Requests.CheckConnection(globalOptions);
        }
        //-------------------------------------------------------------------------------------
        public IEnumerable<Clinic.DataModel.Message> CheckNewMessages()
        {
            List<Clinic.DataModel.Message> result = Requests.PostMessagesHistoryByIdChat(
                ConfigurationManager.AppSettings["HomeWebHook"], ConfigurationManager.AppSettings["HomeWebHookKeyGUI"], m_GlobalOptions);
            return result;
        }
        //-------------------------------------------------------------------------------------
        public bool CheckPhoneNumberInSourceType(string _SourceTypeUID, string _Phone, out string _Result)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения клиента! Нет данных авторизации!");
                _Result = null;
                return false;
            }
            HttpStatusCode? code = Requests.PostCheckPhone(globalOptions, m_AuthInfo, Convert.ToInt64(_SourceTypeUID), _Phone);
            if (code == null)
            {
                _Result = null;
                return false;
            }
            else if (code.Value == HttpStatusCode.OK)
            {
                _Result = "200";
                return true;
            }
            else
            {
                _Result = ((int)code.Value).ToString();
                return false;
            }
        }
        //-------------------------------------------------------------------------------------
        public string FirstSendMessage(string idMessengerAccount, string loginOrPhone, string text,
            string customId, string pathMedia, MessengersType? messengersType = null)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка отправки сообщений! Нет данных авторизации!");
                return null;
            }

            AttachmentForMessage attachment = null;
            if (!String.IsNullOrEmpty(pathMedia) && messengersType != null)
            {
                CMUmnicoEnums.MessengerTypes? type = GetUmnicoEnum((MessengersType)messengersType);
                if (type == null)
                {
                    m_LogDelegate.Invoke("Ошибка отправки сообщений! Не указан тип мессенджера!");
                    return null;
                }
                attachment = UnloadFileForMessage(Convert.ToInt64(idMessengerAccount), true, pathMedia, (CMUmnicoEnums.MessengerTypes)type);
            }

            long messengerId = Convert.ToInt64(idMessengerAccount);

            List<Message> sentMessages = ExecuteWithInitUnauthorizationMessenger(
                () =>
                Requests.PostFirstSendMessagesByIdMessenger(globalOptions, m_AuthInfo, out m_LastException,
                    messengerId, loginOrPhone, text, customId, attachment)
            );

            if (sentMessages != null)
            {
                return attachment?.url ?? "";
            }

            return null;
        }
        //-------------------------------------------------------------------------------------
        public Clinic.DataModel.Customer GetCustomer(string idCustomer)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения клиента! Нет данных авторизации!");
                return null;
            }
            Customer customer = Requests.GetCustomer(globalOptions, m_AuthInfo, Convert.ToInt32(idCustomer));
            Clinic.DataModel.Customer result = new Clinic.DataModel.Customer()
            {
                Id = customer.id.ToString(),
                Avatar = GetBase64FromWebData(customer.avatar),
                Email = customer.email,
                Login = customer.login,
                Name = customer.name,
                Phone = customer.phone
            };
            return result;
        }
        //-------------------------------------------------------------------------------------
        public IEnumerable<Clinic.DataModel.Manager> GetManagers()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения списка операторов! Нет данных авторизации!");
                return null;
            }
            List<Clinic.DataModel.Manager> result = new List<Clinic.DataModel.Manager>();
            foreach (Manager manag in Requests.GetManagers(globalOptions, m_AuthInfo))
            {
                result.Add(new Clinic.DataModel.Manager() { Id = manag.id.ToString(), Login = manag.login, Name = manag.name });
            }
            return result;
        }
        //-------------------------------------------------------------------------------------
        public IEnumerable<SourceMessage> GetSourceMessages()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения интегрированных мессенджеров! Нет данных авторизации!");
                return null;
            }

            List<SourceMessage> result = new List<SourceMessage>();
            List<Messenger> receivedMessengers = new List<Messenger>();
            List<Messenger> temp = null;

            try
            {
                temp = ExecuteWithInitUnauthorizationMessenger(
                    () =>
                    Requests.GetMessengers(globalOptions, m_AuthInfo, out m_LastException)
                );
            }
            catch (Exception ex)
            {
                SaveExceptionToLog(ex, "CMUmnico");
            }
            finally
            {
                if (temp != null)
                {
                    receivedMessengers = temp;
                }
            }

            foreach (Messenger mess in receivedMessengers)
            {
                var type = CMUmnicoEnums.GetMessengerTypes((CMUmnicoEnums.MessengerTypes)Enum.Parse(typeof(CMUmnicoEnums.MessengerTypes), mess.type));
                if (type != null)
                {
                    result.Add(new SourceMessage()
                    {
                        Id = mess.id.ToString(),
                        Login = mess.login,
                        Type = (SiMed.Clinic.DataModel.MessengersType)type
                    });
                }

            }

            return result;
        }
        //-------------------------------------------------------------------------------------
        public bool Init(Clinic.DataModel.ClinicChatAggregatorApplicationType _source)
        {
            m_AuthInfo = Requests.PostAuthorization(globalOptions);
            m_appType = _source;
            m_CheckAuthorization = true;
            return true;
        }
        //-------------------------------------------------------------------------------------
        public void SaveExceptionToLog(Exception ex, string source)
        {
            ApplicationLog.SaveExceptionToLog(ex, m_appType);
        }
        //-------------------------------------------------------------------------------------
        public string SendMessage(string idDialog, string text, string customId, string pathMedia = null, MessengersType? messengersType = null)
        {
            if (m_CheckAuthorization == false || m_CheckLogin == false)
            {
                m_LogDelegate.Invoke("Ошибка отправки сообщений! Нет данных авторизации или сотрудника!");
                return null;
            }

            AttachmentForMessage attachment = null;
            if (!String.IsNullOrEmpty(pathMedia) && messengersType != null)
            {
                CMUmnicoEnums.MessengerTypes? type = GetUmnicoEnum((MessengersType)messengersType);
                if (type == null)
                {
                    m_LogDelegate.Invoke("Ошибка отправки сообщений! Не указан тип мессенджера!");
                    return null;
                }
                attachment = UnloadFileForMessage(Convert.ToInt64(idDialog.Split('\n')[1]), false, pathMedia, (CMUmnicoEnums.MessengerTypes)type);
                if (attachment == null)
                {
                    return null;
                }
            }

            long appealId = Convert.ToInt64(idDialog.Split('\n')[0]);
            string chatId = idDialog.Split('\n')[1];

            ExecuteWithInitUnauthorizationMessenger(
                () =>
                Requests.PutAppeal(globalOptions, m_AuthInfo, m_Manager, appealId, out m_LastException)
            );

            List<Message> sentMessages = ExecuteWithInitUnauthorizationMessenger(
                () =>
                Requests.PostSendMessagesByIdChat(globalOptions, m_AuthInfo, m_Manager, out m_LastException,
                    appealId, chatId, text, customId, attachment)
            );

            if (sentMessages != null)
            {
                return attachment?.url ?? "";
            }

            return null;
        }
        //-------------------------------------------------------------------------------------
        public bool SetOptions(string _SystemOptions, string _LocalOptions)
        {
            try
            {
                globalOptions = (CMUmnicoGlobalOptions)globalOptions.Unpack(_SystemOptions);
                if (_LocalOptions != null)
                {
                    localOptions = (CMUmnicoLocalOptions)localOptions.Unpack(_LocalOptions);
                }
                m_Manager = localOptions.m_Manager;
                m_CheckLogin = m_Manager == null ? false : true;
                return true;
            }
            catch (Exception e)
            {
                m_LogDelegate.Invoke("Ошибка при установке опций чат мессенджера: " + e.Message);
                return false;
            }
        }
        //-------------------------------------------------------------------------------------
        public bool ShowLocalOptions(ref string _LocalOptions)
        {
            CMUmnicoLocalOptionsForm form = new CMUmnicoLocalOptionsForm(_LocalOptions, m_GlobalOptions);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _LocalOptions = form.localOptions.Pack();

                return true;
            }
            else
                return false;
        }
        //-------------------------------------------------------------------------------------
        public bool ShowSystemOptions(ref string _SystemOptions)
        {
            CMUmnicoGlobalOptionsForm form = new CMUmnicoGlobalOptionsForm(_SystemOptions);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _SystemOptions = form.globalOptions.Pack();

                return true;
            }
            else
                return false;
        }
        //-------------------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------------------
        public bool UpdateTokens()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка обновления токена! Нет данных авторизации!");
                return false;
            }
            m_AuthInfo = Requests.UpdateTokens(globalOptions,m_AuthInfo);
            m_CheckAuthorization = m_AuthInfo==null ? false : true;
            return m_CheckAuthorization;
        }
        //-------------------------------------------------------------------------------------
        public List<Appeal> GetAppealsNew()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения новых чатов! Нет данных авторизации!");
                return null;
            }
            
            return Requests.GetAppeals(globalOptions, m_AuthInfo, CMUmnicoEnums.AppealsType.New);
        }
        //-------------------------------------------------------------------------------------
        public List<Appeal> GetAppealsActive()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения активных чатов! Нет данных авторизации!");
                return null;
            }

            return Requests.GetAppeals(globalOptions, m_AuthInfo, CMUmnicoEnums.AppealsType.Active);
        }
        //-------------------------------------------------------------------------------------
        public List<Appeal> GetAppealsArchive()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения архивных чатов! Нет данных авторизации!");
                return null;
            }

            return Requests.GetAppeals(globalOptions, m_AuthInfo, CMUmnicoEnums.AppealsType.Archive);
        }
        //-------------------------------------------------------------------------------------
        public List<Appeal> GetAppealsAll()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения всех чатов! Нет данных авторизации!");
                return null;
            }

            return Requests.GetAppeals(globalOptions, m_AuthInfo, CMUmnicoEnums.AppealsType.All);
        }
        //-------------------------------------------------------------------------------------
        public Appeal GetAppealOne(int id)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения одного чата! Нет данных авторизации!");
                return null;
            }

            return Requests.GetAppealOne(globalOptions, m_AuthInfo, id);
        }
        //-------------------------------------------------------------------------------------
        public List<Appeal> GetAppealsById(List<long> ids)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка получения чатов по id! Нет данных авторизации!");
                return null;
            }
            return Requests.GetAppealsById(globalOptions, m_AuthInfo, ids);
        }
        //-------------------------------------------------------------------------------------
        public bool ReadMessageForAppeal(int id)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke( "Ошибка изменения статуса сообщения по id чата! Нет данных авторизации!");
                return false;
            }
            return Requests.PutStateMessageForAppeal(globalOptions, m_AuthInfo, id);
        }
        //-------------------------------------------------------------------------------------
        public bool AcceptAppealByIdManager(int id)
        {
            if (m_CheckAuthorization == false || m_CheckLogin == false)
            {
                m_LogDelegate.Invoke( "Ошибка принятия обращения в работу по id сотрудника и id обращения! Нет данных авторизации или сотрудника!");
                return false;
            }
            return Requests.PutAcceptAppealByIdManager(globalOptions, m_AuthInfo,m_Manager, id);
        }
        //-------------------------------------------------------------------------------------
        public bool ChangeAppeal(List<KeyValuePair<string,object>> listKeyValuem, long id)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка изменения обращения! Нет данных авторизации!");
                return false;
            }
            Requests.ChangeAppeal changeAppeal = new Requests.ChangeAppeal();
            foreach (KeyValuePair<string, object> keyValue in listKeyValuem)
            {
                if (keyValue.Key == "statusId")
                {
                    changeAppeal.statusId = Convert.ToInt32(keyValue.Value);
                }
                else if (keyValue.Key == "details")
                {
                    changeAppeal.details = keyValue.Value.ToString();
                }
                else if (keyValue.Key == "userId")
                {
                    changeAppeal.userId = Convert.ToInt64(keyValue.Value);
                }
                else if (keyValue.Key == "address")
                {
                    changeAppeal.address = keyValue.Value.ToString();
                }
            }
            return Requests.PutChangeAppeal(globalOptions, m_AuthInfo, changeAppeal, id);
        }
        //-------------------------------------------------------------------------------------
        public List<Chat> GetChatsByIdAppeal(int id)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения каналов! Нет данных авторизации!");
                return null;
            }
            return Requests.GetChatsByIdAppeal(globalOptions, m_AuthInfo,id);
        }
        //-------------------------------------------------------------------------------------
        public AttachmentForMessage UnloadFileForMessage(long id, bool first, string pathMedia, CMUmnicoEnums.MessengerTypes mesTypes)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка загрузки файла! Нет данных авторизации!");
                return null;
            }
            string fileName = Path.GetFileName(pathMedia);
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string contentType = MimeType.GetMimeType(fileName.Split('.')[1]);
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";
            MemoryStream memoryStream = new MemoryStream();
            
            if (first)
            {
                WriteToStream(memoryStream, boundarybytes, string.Format(formdataTemplate, "saId", id.ToString()));
            }
            else
            {
                WriteToStream(memoryStream, boundarybytes, string.Format(formdataTemplate, "source", id.ToString()));
            }
            WriteToStream(memoryStream, boundarybytes, string.Format(fileheaderTemplate, "media", fileName, contentType), false);

            using (FileStream fstream = new FileStream(pathMedia,FileMode.Open,FileAccess.Read))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                WriteToStream(memoryStream, new byte[0], buffer, false);
            }
            WriteToStream(memoryStream, new byte[0], trailer);

            return Requests.PostUploadFile(globalOptions, m_AuthInfo, memoryStream, boundary, mesTypes);
        }
        //-------------------------------------------------------------------------------------
        public bool DeleteWebHook(int idWebHook)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка удаления хука! Нет данных авторизации!");
                return false;
            }

            return Requests.DeleteWebHook(globalOptions, m_AuthInfo, idWebHook);
        }
        //-------------------------------------------------------------------------------------
        public List<WebHook> GetWebHooks()
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка получения хуков! Нет данных авторизации!");
                return null;
            }
            return Requests.GetWebHooks(globalOptions, m_AuthInfo);
        }
        //-------------------------------------------------------------------------------------
        public WebHook AddWebHook(string _url)
        {
            if (m_CheckAuthorization == false)
            {
                m_LogDelegate.Invoke("Ошибка добавления хуков! Нет данных авторизации!");
                return null;
            }
           
            return Requests.AddWebHook(globalOptions, m_AuthInfo, _url);
        }
        //-------------------------------------------------------------------------------------
        #region Вспомогательные закрытые методы
        //-------------------------------------------------------------------------------------
        private CMUmnicoEnums.MessengerTypes? GetUmnicoEnum(MessengersType messengersType)
        {
            CMUmnicoEnums.MessengerTypes? type = null;
            if (messengersType == MessengersType.WhatsApp)
            {
                type = CMUmnicoEnums.MessengerTypes.whatsapp2;
            }
            else if (messengersType == MessengersType.Vk)
            {
                type = CMUmnicoEnums.MessengerTypes.vk_group;
            }
            else if (messengersType == MessengersType.Viber)
            {
                type = CMUmnicoEnums.MessengerTypes.viber_bot;
            }
            else if (messengersType == MessengersType.Twitter)
            {
                type = CMUmnicoEnums.MessengerTypes.other;
            }
            else if (messengersType == MessengersType.Telegram)
            {
                type = CMUmnicoEnums.MessengerTypes.telegram;
            }
            else if (messengersType == MessengersType.Sms)
            {
                type = CMUmnicoEnums.MessengerTypes.other;
            }
            else if (messengersType == MessengersType.Phone)
            {
                type = CMUmnicoEnums.MessengerTypes.other;
            }
            else if (messengersType == MessengersType.Ok)
            {
                type = CMUmnicoEnums.MessengerTypes.ok;
            }
            else if (messengersType == MessengersType.Facebook)
            {
                type = CMUmnicoEnums.MessengerTypes.fb_messenger;
            }
            else if (messengersType == MessengersType.Email)
            {
                type = CMUmnicoEnums.MessengerTypes.mailbox;
            }
            else if (messengersType == MessengersType.Chat)
            {
                type = CMUmnicoEnums.MessengerTypes.other;
            }
            else if (messengersType == MessengersType.Avito)
            {
                type = CMUmnicoEnums.MessengerTypes.avito;
            }
            else if (messengersType == MessengersType.Instagram)
            {
                type = CMUmnicoEnums.MessengerTypes.instagramV3;
            }
            return type;
        }
        //-------------------------------------------------------------------------------------
        private static void WriteToStream(Stream s, byte[] boundarybytes, string txt, bool WriteCR = true)
        {
            byte[] bytesCR = Encoding.UTF8.GetBytes("\r\n");
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(boundarybytes, 0, boundarybytes.Length);
            s.Write(bytes, 0, bytes.Length);
            if (WriteCR)
                s.Write(bytesCR, 0, bytesCR.Length);
        }
        //-------------------------------------------------------------------------------------
        private static void WriteToStream(Stream s, byte[] boundarybytes, byte[] bytes, bool WriteCR = true)
        {
            byte[] bytesCR = Encoding.UTF8.GetBytes("\r\n");
            s.Write(boundarybytes, 0, boundarybytes.Length);
            s.Write(bytes, 0, bytes.Length);
            if (WriteCR)
                s.Write(bytesCR, 0, bytesCR.Length);
        }
        //-------------------------------------------------------------------------------------
        private string GetBase64FromWebData(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            //request.ContentType = MimeType.GetMimeType(uri.Segments[uri.Segments.Length-1].Split('.')[1]);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream reader = new MemoryStream())
                    {
                        stream.CopyTo(reader);
                        byte[] dataUser = reader.ToArray();
                        return Convert.ToBase64String(dataUser);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveExceptionToLog(ex, "CMUmnico");
                m_LogDelegate.Invoke("Ошибка получения base64" + ex.Message);
                return null;
            }
        }
        //-------------------------------------------------------------------------------------
        private void ExecuteWithInitUnauthorizationMessenger(Action _Action)
        {
            _Action.Invoke();

            if (m_LastException is WebException exception)
            {
                HttpWebResponse response = (HttpWebResponse)exception.Response;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Init(m_appType);
                    _Action.Invoke();
                }
            }
        }
        //-------------------------------------------------------------------------------------
        private TResult ExecuteWithInitUnauthorizationMessenger<TResult>(Func<TResult> _Func)
        {
            TResult result = _Func.Invoke();

            if (m_LastException is WebException exception)
            {
                HttpWebResponse response = (HttpWebResponse)exception.Response;
                if (response?.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Init(m_appType);
                    result = _Func.Invoke();
                }
            }

            return result;
        }
        //-------------------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------------------
    }
}

