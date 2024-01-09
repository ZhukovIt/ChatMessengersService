using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows.Forms;

namespace SiMed.ChatMessengers.Umnico
{
    public class Requests
    {
        public class ChangeAppeal
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int statusId;
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string details;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public long userId;
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string address;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <returns></returns>
        public static bool CheckConnection(CMUmnicoGlobalOptions _globalOptions)
        {
            Uri uri = new Uri(_globalOptions.MAIN_URL);
            string url = uri.GetLeftPart(UriPartial.Authority);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url+"/");
            webRequest.Method = "GET";
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalOptions"></param>
        /// <returns></returns>
        public static AuthorizationInfo PostAuthorization(CMUmnicoGlobalOptions globalOptions)
        {
            string _URL = globalOptions.MAIN_URL + "/auth/login";
            string _login = globalOptions.LOGIN;
            string _password = globalOptions.PASSWORD;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            // данные для отправки
            object jsondata = new { login = _login , pass = _password };
            string data = JsonConvert.SerializeObject(jsondata);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            //записываем данные в поток запроса
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JObject.Parse(dataUser);
                            return new AuthorizationInfo(jObject["accessToken"]["token"].Value<string>(), jObject["refreshToken"]["token"].Value<string>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <returns></returns>
        public static AuthorizationInfo UpdateTokens(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo)
        {
            string _URL = _globalOptions.MAIN_URL + "/auth/tokens";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            HttpWebResponse response=null;
            try
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JObject.Parse(dataUser);
                            return new AuthorizationInfo(jObject["accessToken"]["token"].Value<string>(), _authInfo.refreshToken);
                        }
                    }
                }
                catch (WebException ex)
                {
                    using (WebResponse _response = ex.Response)
                    {
                        HttpWebResponse httpExResponse = (HttpWebResponse)_response;
                        if (httpExResponse.StatusCode == HttpStatusCode.Unauthorized && _authInfo.accessToken!= _authInfo.refreshToken)
                        {
                            _authInfo.accessToken = _authInfo.refreshToken;
                            return UpdateTokens(_globalOptions, _authInfo);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка обновления токенов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <returns></returns>
        public static List<Messenger> GetMessengers(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo,
            out Exception _Exception)
        {
            _Exception = null;
            string _URL = _globalOptions.MAIN_URL + "/integrations";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            string dataUser = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Messenger> listMessengers = new List<Messenger>();
                        foreach (JObject value in jObject)
                        {
                            listMessengers.Add(new Messenger()
                            {
                                id = value["id"].Value<int>(),
                                type = value["type"].Value<string>(),
                                avatar = value["avatar"].Value<string>(),
                                externalId = value["externalId"].Value<string>(),
                                identifier = value["identifier"].Value<string>(),
                                login = value["login"].Value<string>(),
                                status = value["status"].Value<string>(),
                                url = value["url"].Value<string>()

                            });
                        }
                        return listMessengers;
                    }
                }
            }
            catch (WebException ex)
            {
                ApplicationLog.SaveExceptionToLog(new Exception("WebException" +" "+dataUser), CMUmnico.m_appType);
                throw;
            }
            catch (Exception ex)
            {
                _Exception = ex;
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                ApplicationLog.SaveExceptionToLog(new Exception(dataUser), CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка интегрированных мессенджеров", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_AppealsType"></param>
        /// <returns></returns>
        public static List<Appeal> GetAppeals(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, CMUmnicoEnums.AppealsType _AppealsType)
        {
            string AppealType = "";
            if (_AppealsType == CMUmnicoEnums.AppealsType.Active)
            {
                AppealType = "active";
            }
            else if(_AppealsType == CMUmnicoEnums.AppealsType.New)
            {
                AppealType = "inbox";
            }
            else if (_AppealsType == CMUmnicoEnums.AppealsType.Archive)
            {
                AppealType = "completed";
            }
            else if (_AppealsType == CMUmnicoEnums.AppealsType.All)
            {
                AppealType = "all";
            }
            string _URL = _globalOptions.MAIN_URL + "/leads/"+AppealType+"?offset=0&limit=10";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Appeal> listAppeals = new List<Appeal>();
                        foreach (JObject value in jObject)
                        {
                            Appeal Appeal = new Appeal();
                            Appeal.id = value["id"].Value<long>();
                            Appeal.userId = value["userId"].Value<long?>();
                            Appeal.statusId = value["statusId"].Value<int?>();
                            Appeal.read = value["read"].Value<bool>();
                            Appeal.amount = value["amount"].Value<long?>();
                            Appeal.details = value["details"].Value<string>();
                            List<string> tags = new List<string>();
                            foreach (JValue value2 in value["tags"])
                            {
                                tags.Add(value2.Value<string>());
                            }
                            Appeal.tags = tags.ToArray();
                            Appeal.responseTime = value["responseTime"].Value<int?>();
                            Appeal.createdAt = value["createdAt"].Value<string>();
                            Appeal.address = value["address"].Value<string>();
                            Appeal.ttn = value["ttn"].Value<string>();
                            Appeal.customData = value["customData"].Value<string>();
                            Appeal.customFields = value["customFields"].Value<string>();
                            Appeal.paymentTypeId = value["paymentTypeId"].Value<int?>();
                            Appeal.socialAccount = new Messenger()
                            {
                                id = value["socialAccount"]["id"].Value<int>(),
                                login = value["socialAccount"]["login"].Value<string>(),
                                type = value["socialAccount"]["type"].Value<string>()
                            };
                            Appeal.customer = new Customer()
                            {
                                id = value["customer"]["id"].Value<int>(),
                                login = value["customer"]["login"].Value<string>(),
                                name = value["customer"]["name"].Value<string>(),
                                avatar = value["customer"]["avatar"].Value<string>(),
                                email = value["customer"]["email"].Value<string>(),
                                phone = value["customer"]["phone"].Value<string>()
                            };
                            LastMessageForAppeals lastMess = new LastMessageForAppeals();


                            lastMess.unread = value["message"]["unread"].Value<int>();
                            lastMess.timestamp = value["message"]["timestamp"].Value<string>();
                            lastMess.incoming = value["message"]["incoming"].Value<bool>();
                                DataForLastMessage dataForLastMess = new DataForLastMessage();
                                dataForLastMess.text = value["message"]["data"]["text"].Value<string>();
                                if (value["message"]["data"]["url"] != null)
                                {
                                    dataForLastMess.url = value["message"]["data"]["url"].Value<string>();
                                }
                                if (value["message"]["data"]["attachments"] != null)
                                {
                                    List<AttachmentForMessage> Attachments = new List<AttachmentForMessage>();
                                    foreach (JObject value2 in value["message"]["data"]["attachments"])
                                    {
                                        Attachments.Add(new AttachmentForMessage() { type = value2["type"].Value<string>(), url = value2["url"].Value<string>(), text = value2["text"]==null ? "": value2["text"].Value<string>() });
                                    }
                                    dataForLastMess.Attachments = Attachments.ToArray();
                                }
                            lastMess.data = dataForLastMess;
                            Appeal.message = lastMess;
                            listAppeals.Add(Appeal);
                        }
                        return listAppeals;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка обращений", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_ids"></param>
        /// <returns></returns>
        public static List<Appeal> GetAppealsById(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, List<long> _ids)
        {
            string Ids = "?&id=";
            foreach (int id in _ids)
            {
                Ids += id + "&id=";
            }
            string _URL = _globalOptions.MAIN_URL + "/leads/" + Ids.Substring(0, Ids.Length-4);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Appeal> listAppeals = new List<Appeal>();
                        foreach (JObject value in jObject)
                        {
                            Appeal Appeal = new Appeal();
                            Appeal.id = value["id"].Value<long>();
                            Appeal.userId = value["userId"].Value<long?>();
                            Appeal.statusId = value["statusId"].Value<int?>();
                            Appeal.read = value["read"].Value<bool>();
                            Appeal.amount = value["amount"].Value<long?>();
                            Appeal.details = value["details"].Value<string>();
                            List<string> tags = new List<string>();
                            foreach (JValue value2 in value["tags"])
                            {
                                tags.Add(value2.Value<string>());
                            }
                            Appeal.tags = tags.ToArray();
                            Appeal.responseTime = value["responseTime"].Value<int?>();
                            Appeal.createdAt = value["createdAt"].Value<string>();
                            Appeal.address = value["address"].Value<string>();
                            Appeal.ttn = value["ttn"].Value<string>();
                            Appeal.customData = value["customData"].Value<string>();
                            Appeal.customFields = value["customFields"].Value<string>();
                            Appeal.paymentTypeId = value["paymentTypeId"].Value<int?>();
                            Appeal.socialAccount = new Messenger()
                            {
                                id = value["socialAccount"]["id"].Value<int>(),
                                login = value["socialAccount"]["login"].Value<string>(),
                                type = value["socialAccount"]["type"].Value<string>()
                            };
                            Appeal.customer = new Customer()
                            {
                                id = value["customer"]["id"].Value<int>(),
                                login = value["customer"]["login"].Value<string>(),
                                name = value["customer"]["name"].Value<string>(),
                                avatar = value["customer"]["avatar"].Value<string>(),
                                email = value["customer"]["email"].Value<string>(),
                                phone = value["customer"]["phone"].Value<string>()
                            };
                            LastMessageForAppeals lastMess = new LastMessageForAppeals();


                            lastMess.unread = value["message"]["unread"].Value<int>();
                            lastMess.timestamp = value["message"]["timestamp"].Value<string>();
                            lastMess.incoming = value["message"]["incoming"].Value<bool>();
                            DataForLastMessage dataForLastMess = new DataForLastMessage();
                            dataForLastMess.text = value["message"]["data"]["text"].Value<string>();
                            if (value["message"]["data"]["url"] != null)
                            {
                                dataForLastMess.url = value["message"]["data"]["url"].Value<string>();
                            }
                            List<AttachmentForMessage> Attachments = new List<AttachmentForMessage>();
                            foreach (JObject value2 in value["message"]["data"]["attachments"])
                            {
                                Attachments.Add(new AttachmentForMessage() { type = value2["type"].Value<string>(), url = value2["url"].Value<string>(), text = value2["text"].Value<string>() });
                            }
                            dataForLastMess.Attachments = Attachments.ToArray();
                            lastMess.data = dataForLastMess;
                            Appeal.message = lastMess;
                            listAppeals.Add(Appeal);
                        }
                        return listAppeals;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка обращений", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static Appeal GetAppealOne(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, long _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/leads/" + _id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var value = JObject.Parse(dataUser);
                        Appeal Appeal = new Appeal();
                        
                        Appeal.id = value["id"].Value<long>();
                        Appeal.userId = value["userId"].Value<long?>();
                        Appeal.statusId = value["statusId"].Value<int?>();
                        Appeal.read = value["read"].Value<bool>();
                        Appeal.amount = value["amount"].Value<long?>();
                        Appeal.details = value["details"].Value<string>();
                        Appeal.responseTime = value["responseTime"].Value<int?>();
                        Appeal.address = value["address"].Value<string>();
                        Appeal.ttn = value["ttn"].Value<string>();
                        Appeal.customData = value["customData"].Value<string>();
                        Appeal.customFields = value["customFields"].Value<string>();
                        if (value["paymentTypeId"]!=null)
                        {
                            Appeal.paymentTypeId = value["paymentTypeId"].Value<int?>();
                        }
                        
                        return Appeal;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения обращения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static bool PutStateMessageForAppeal(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, long _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/leads/" + _id+"/read";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_manager"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static bool PutAcceptAppealByIdManager(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, Manager _manager, long _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/leads/" + _id + "/accept";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            object jsondata = new { userId = _manager.id };
            string data = JsonConvert.SerializeObject(jsondata);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                }
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <returns></returns>
        public static List<Manager> GetManagers(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo)
        {
            string _URL = _globalOptions.MAIN_URL + "/managers";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Manager> listManagers = new List<Manager>();
                        foreach (JObject value in jObject)
                        {
                            Manager manager = new Manager();
                            manager.id = value["id"].Value<int>();
                            manager.name = value["name"].Value<string>();
                            manager.login = value["login"].Value<string>();
                            manager.role = value["role"].Value<string>();
                            manager.confirmed = value["confirmed"].Value<bool>();
                            manager.allowAllDeals = value["allowAllDeals"].Value<bool>();
                            List<AvailableMessenger> sources = new List<AvailableMessenger>();
                            foreach (JObject value2 in value["sources"])
                            {
                                sources.Add(new AvailableMessenger() { id = value2["id"].Value<int>(), messenger = new Messenger() { id = value2["saId"].Value<int>() } });
                            }
                            manager.sources = sources;
                            listManagers.Add(manager);
                        }
                        return listManagers;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка сотрудников", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <returns></returns>
        public static Customer GetCustomer(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, long id)
        {
            string _URL = _globalOptions.MAIN_URL + "/customers/"+id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JObject.Parse(dataUser);
                        Customer customer = jObject.ToObject<Customer>();
                        
                        return customer;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения клиента", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_login"></param>
        /// <param name="_listMessengers"></param>
        /// <returns></returns>
        public static List<Manager> PostAddManager(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, string _login, List<object> _listMessengers)
        {
            string _URL = _globalOptions.MAIN_URL + "/managers";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            object jsondata = new { login = _login, sources = _listMessengers.ToArray() };
            string data = JsonConvert.SerializeObject(jsondata);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                 using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Manager> listManagers = new List<Manager>();
                        foreach (JObject value in jObject)
                        {
                            Manager manager = new Manager();
                            manager.id = value["id"].Value<int>();
                            manager.name = value["name"].Value<string>();
                            manager.login = value["login"].Value<string>();
                            manager.role = value["role"].Value<string>();
                            manager.confirmed = value["confirmed"].Value<bool>();
                            manager.allowAllDeals = value["allowAllDeals"].Value<bool>();
                            listManagers.Add(manager);
                        }
                        return listManagers;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_changeAppeal"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static bool PutChangeAppeal(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo,ChangeAppeal _changeAppeal, long _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/leads/" + _id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            string data = JsonConvert.SerializeObject(_changeAppeal);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                }
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static List<Chat> GetChatsByIdAppeal(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, long _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/messaging/"+_id+"/sources";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<Chat> listChats = new List<Chat>();
                        foreach (JObject value in jObject)
                        {
                            Chat Chat = new Chat();

                            Chat.id = value["id"].Value<string>();
                            Chat.name = value["name"].Value<string>();
                            Chat.type = value["type"].Value<string>();
                            Chat.expires = value["expires"].Value<DateTime?>();
                            Chat.identifier = value["identifier"].Value<string>();
                            if (value["realId"]!=null)
                            {
                                Chat.realId = value["realId"].Value<int>();
                            }
                            else
                            {
                                Chat.realId = value["id"].Value<int>();
                            }
                            Chat.saId = value["saId"].Value<int>();
                            Chat.token = value["token"].Value<string>();
                            Chat.sender = value["sender"].Value<string>();

                            listChats.Add(Chat);
                        }
                        return listChats;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка чатов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static List<SiMed.Clinic.DataModel.Message> PostMessagesHistoryByIdChat(string url, string key, CMUmnicoGlobalOptions options)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url+ "/HomeApi/ChatMessengersWebHooks/GetMessages?login="+ HttpUtility.UrlEncode(options.LOGIN));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "GUI_" + key;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        List <SiMed.Clinic.DataModel.Message> listMessages = (JArray.Parse(dataUser)).ToObject<List<SiMed.Clinic.DataModel.Message>>();
                        return listMessages;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка сообщений", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_manager"></param>
        /// <param name="_idAppeal"></param>
        /// <param name="_idChat"></param>
        /// <param name="_text"></param>
        /// <param name="_attachment"></param>
        /// <returns></returns>
        public static List<Message> PostSendMessagesByIdChat(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, Manager _manager, out Exception exc, long _idAppeal, string _idChat, string _text,string _customId, AttachmentForMessage _attachment=null)
        {
            string _URL = _globalOptions.MAIN_URL + "/messaging/" + _idAppeal + "/send";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            object messageObj=null;
            if (_attachment == null)
            {
                messageObj = new { message = new { text = _text }, source = _idChat, userId = _manager.id, customId = _customId };
            }
            else
            {

                messageObj = new { message = new { text = _text, attachment = _attachment }, source = _idChat, userId = _manager.id, customId = _customId };
            }
            
            string data = JsonConvert.SerializeObject(messageObj);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JArray.Parse(dataUser);

                            List<Message> listMessages = new List<Message>();
                            foreach (JObject value in jObject)
                            {
                                Message message = new Message();
                                if (value["datetime"] != null)
                                {
                                    message.datetime = value["datetime"].Value<long>();
                                }
                                message.sa = new Messenger() { id = value["sa"]["id"].Value<int>(), type = value["sa"]["type"].Value<string>(), login = value["sa"]["login"].Value<string>() };
                                if (value["message"] != null)
                                {
                                    message.message = new MessageItem();
                                    message.message.text = value["message"]["text"].Value<string>();
                                    if (value["message"]["url"] != null)
                                    {
                                        message.message.url = value["message"]["url"].Value<string>();
                                    }
                                    if (value["message"]["attachments"] != null)
                                    {
                                        List<AttachmentForMessage> Attachments = new List<AttachmentForMessage>();
                                        foreach (JObject value2 in value["message"]["attachments"])
                                        {
                                            Attachments.Add(value2.ToObject<AttachmentForMessage>());
                                        }
                                        message.message.attachments = Attachments.ToArray();
                                    }
                                    if (value["message"]["replyTo"] != null)
                                    {
                                        List<AttachmentForMessage> Attachments2 = new List<AttachmentForMessage>();
                                        foreach (JObject value2 in value["message"]["replyTo"]["message"]["attachments"])
                                        {
                                            Attachments2.Add(new AttachmentForMessage() { type = value2["type"].Value<string>(), url = value2["url"].Value<string>(), text = value2["text"].Value<string>(), preview = value2["preview"].Value<string>(), filesize = value2["filesize"].Value<float>() });
                                        }
                                        message.replyTo = new ReplyMessage()
                                        {
                                            datetime = value["message"]["replyTo"]["datetime"].Value<int>(),
                                            message = new MessageItem()
                                            {
                                                text = value["message"]["replyTo"]["message"]["text"].Value<string>(),
                                                url = value["message"]["replyTo"]["message"]["url"].Value<string>(),
                                                attachments = Attachments2.ToArray()
                                            },
                                            messageId = value["message"]["replyTo"]["messageId"].Value<string>(),
                                            incoming = value["message"]["replyTo"]["incoming"].Value<bool>()
                                        };
                                    }
                                }
                                if (value["preview"] != null)
                                {
                                    message.preview = value["preview"].ToObject<DescriptionPost>();
                                }
                                if (value["source"] != null)
                                {
                                    message.source = value["source"].ToObject<Chat>();
                                }
                                if (value["customId"] != null)
                                {
                                    message.customId = value["customId"].Value<string>();
                                }
                                if (value["messageId"] != null)
                                {
                                    message.messageId = value["messageId"].Value<string>();
                                }
                                if (value["sender"] != null)
                                {
                                    message.sender = value["sender"].ToObject<Author>();
                                }
                                if (value["incoming"] != null)
                                {
                                    message.incoming = value["incoming"].Value<bool>();
                                }

                                listMessages.Add(message);
                            }
                            exc = null;
                            return listMessages;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                exc = ex;
                //MessageBox.Show("" + ex.Message, "Ошибка отправки сообщения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_idMessenger"></param>
        /// <param name="_text"></param>
        /// <param name="_attachment"></param>
        /// <returns></returns>
        public static List<Message> PostFirstSendMessagesByIdMessenger(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, out Exception exc, long _idMessenger,string _loginOrPhone, string _text, string _customId, AttachmentForMessage _attachment = null)
        {
            string _URL = _globalOptions.MAIN_URL + "/messaging/post";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            object messageObj = null;
            if (_attachment == null)
            {
                messageObj = new { message = new { text = _text }, saId = _idMessenger, destination = _loginOrPhone, customId = _customId };
            }
            else
            {
                messageObj = new { message = new { text = _text, attachment = _attachment }, saId = _idMessenger, destination = _loginOrPhone, customId = _customId };
            }

            string data = JsonConvert.SerializeObject(messageObj);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JArray.Parse(dataUser);

                            List<Message> listMessages = new List<Message>();
                            foreach (JObject value in jObject)
                            {
                                Message message = new Message();
                                if (value["datetime"] != null)
                                {
                                    message.datetime = value["datetime"].Value<long>();
                                }

                                message.sa = new Messenger() { id = value["sa"]["id"].Value<int>(), type = value["sa"]["type"].Value<string>(), login = value["sa"]["login"].Value<string>() };
                                if (value["message"] != null)
                                {
                                    message.message = new MessageItem();
                                    message.message.text = value["message"]["text"].Value<string>();
                                    if (value["message"]["url"] != null)
                                    {
                                        message.message.url = value["message"]["url"].Value<string>();
                                    }
                                    if (value["message"]["attachments"] != null)
                                    {
                                        List<AttachmentForMessage> Attachments = new List<AttachmentForMessage>();
                                        foreach (JObject value2 in value["message"]["attachments"])
                                        {
                                            Attachments.Add(new AttachmentForMessage() { type = value2["type"].Value<string>(), url = value2["url"].Value<string>(), text = value2["text"].Value<string>(), preview = value2["preview"].Value<string>(), filesize = value2["filesize"].Value<float>() });
                                        }
                                        message.message.attachments = Attachments.ToArray();
                                    }
                                    if (value["message"]["replyTo"] != null)
                                    {
                                        List<AttachmentForMessage> Attachments2 = new List<AttachmentForMessage>();
                                        foreach (JObject value2 in value["message"]["replyTo"]["message"]["attachments"])
                                        {
                                            Attachments2.Add(new AttachmentForMessage() { type = value2["type"].Value<string>(), url = value2["url"].Value<string>(), text = value2["text"].Value<string>(), preview = value2["preview"].Value<string>(), filesize = value2["filesize"].Value<float>() });
                                        }
                                        message.replyTo = new ReplyMessage()
                                        {
                                            datetime = value["message"]["replyTo"]["datetime"].Value<int>(),
                                            message = new MessageItem()
                                            {
                                                text = value["message"]["replyTo"]["message"]["text"].Value<string>(),
                                                url = value["message"]["replyTo"]["message"]["url"].Value<string>(),
                                                attachments = Attachments2.ToArray()
                                            },
                                            messageId = value["message"]["replyTo"]["messageId"].Value<string>(),
                                            incoming = value["message"]["replyTo"]["incoming"].Value<bool>()
                                        };
                                    }
                                }
                                if (value["preview"] != null)
                                {
                                    message.preview = value["preview"].ToObject<DescriptionPost>();
                                }
                                if (value["source"] != null)
                                {
                                    message.source = value["source"].ToObject<Chat>();
                                }
                                if (value["customId"] != null)
                                {
                                    message.customId = value["customId"].Value<string>();
                                }
                                if (value["messageId"] != null)
                                {
                                    message.messageId = value["messageId"].Value<string>();
                                }
                                if (value["sender"] != null)
                                {
                                    message.sender = value["sender"].ToObject<Author>();
                                }
                                if (value["incoming"] != null)
                                {
                                    message.incoming = value["incoming"].Value<bool>();
                                }

                                listMessages.Add(message);
                            }
                            exc = null;
                            return listMessages;
                        }
                    }
                }
            }
            catch (Exception ex)
            {              
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                exc = ex;
                //MessageBox.Show("" + ex.Message, "Ошибка отправки сообщения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_id"></param>
        /// <param name="_first"></param>
        /// <param name="_media"></param>
        /// <returns></returns>
        public static AttachmentForMessage PostUploadFile(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, MemoryStream _media, string _boundary, CMUmnicoEnums.MessengerTypes _messengerTypes)
        {
            string _URL = _globalOptions.MAIN_URL + "/messaging/upload";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary="+ _boundary;
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    byte[] buffer = _media.ToArray();
                    dataStream.Write(buffer, 0, buffer.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JObject.Parse(dataUser);

                            AttachmentForMessage attachment = new AttachmentForMessage();
                            attachment.type = jObject["type"].Value<string>();
                            if (_messengerTypes == CMUmnicoEnums.MessengerTypes.vk_group)
                            {
                                attachment.media = jObject["media"].ToObject<MediaVk>();
                                attachment.url = ((MediaVk)attachment.media).url;
                            }
                            else if (_messengerTypes == CMUmnicoEnums.MessengerTypes.whatsapp2)
                            {
                                attachment.media = jObject["media"].ToObject<MediaWhatsapp>();
                                attachment.url = ((MediaWhatsapp)attachment.media).src;
                            }
                            else if (_messengerTypes == CMUmnicoEnums.MessengerTypes.telegram)
                            {
                                attachment.media = jObject["media"].ToObject<MediaTelegram>();
                                attachment.url = "";
                            }
                            else if (_messengerTypes == CMUmnicoEnums.MessengerTypes.instagramV3 || _messengerTypes == CMUmnicoEnums.MessengerTypes.viber_bot || _messengerTypes == CMUmnicoEnums.MessengerTypes.ok)
                            {
                                attachment.media = jObject["media"].ToObject<MediaInstagramViberOK>();
                                attachment.url = ((MediaInstagramViberOK)attachment.media).url;
                            }
                            else
                            {
                                attachment.media = jObject["media"].ToObject<MediaOther>();
                                attachment.url = "";
                            }
                            return attachment;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка загрузки файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        //
        public static List<WebHook> GetWebHooks(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo)
        {
            string _URL = _globalOptions.MAIN_URL + "/webhooks/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string dataUser = reader.ReadToEnd();
                        var jObject = JArray.Parse(dataUser);
                        List<WebHook> listWebHooks = new List<WebHook>();
                        foreach (JObject value in jObject)
                        {
                            listWebHooks.Add(value.ToObject<WebHook>());
                        }
                        return listWebHooks;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка получения списка Веб-хуков", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        //
        public static WebHook AddWebHook(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, string _url)
        {
            string _URL = _globalOptions.MAIN_URL + "/webhooks/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            string data = JsonConvert.SerializeObject(new { url =_url });
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string dataUser = reader.ReadToEnd();
                            var jObject = JObject.Parse(dataUser);
                            WebHook webHook = jObject.ToObject<WebHook>();
                            return webHook;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                MessageBox.Show("" + ex.Message, "Ошибка получения списка Веб-хуков", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        //
        public static bool AddClinicForServer(string url, string guid, string id, string login)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "/HomeApi/ChatMessengersWebHooks/CreateOrg");

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "GUI_" + guid;

            string data = JsonConvert.SerializeObject(new { login = login, id = id });
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                }
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                MessageBox.Show("" + ex.Message, "Ошибка создания организации на сервере Клиники", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        //
        public static bool DeleteWebHook(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, int _id)
        {
            string _URL = _globalOptions.MAIN_URL + "/webhooks/"+ _id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                MessageBox.Show("" + ex.Message, "Ошибка получения списка Веб-хуков", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        //
        public static bool PutAppeal(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, Manager _manager, long _idAppeal,
            out Exception _Exception)
        {
            _Exception = null;
            string _URL = _globalOptions.MAIN_URL + "/leads/"+ _idAppeal + "/accept";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            string data = JsonConvert.SerializeObject(new { userId = _manager.id });
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                }
                return true;
            }
            catch (Exception ex)
            {
                _Exception = ex;
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка принятия обращения (назначение оператора)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_globalOptions"></param>
        /// <param name="_authInfo"></param>
        /// <param name="_idMessenger"></param>
        /// <param name="_loginOrPhone"></param>
        /// <param name="_text"></param>
        /// <param name="_customId"></param>
        /// <param name="_attachment"></param>
        /// <returns></returns>
        public static HttpStatusCode? PostCheckPhone(CMUmnicoGlobalOptions _globalOptions, AuthorizationInfo _authInfo, long _idMessenger, string _loginOrPhone)
        {
            string _URL = _globalOptions.MAIN_URL + "/messaging/check-contact";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = _authInfo.accessToken;

            object messageObj = new { saId = _idMessenger, chatId = "7"+_loginOrPhone };


            string data = JsonConvert.SerializeObject(messageObj);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    return response.StatusCode;
                } 
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return HttpStatusCode.NotFound;
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    return HttpStatusCode.BadRequest;
                }
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                //MessageBox.Show("" + ex.Message, "Ошибка проверки телефона", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static bool CheckConnectionSiMed(string _url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.Method = "GET";
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.SaveExceptionToLog(ex, CMUmnico.m_appType);
                return false;
            }
        }
        //--------------------------------------------------------------------------------
    }
}

