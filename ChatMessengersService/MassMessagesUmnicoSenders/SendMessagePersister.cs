using BulkMessagesWebServer.DataModel;
using ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class SendMessagePersister
    {
        private IMessengerCommon m_ChatMessenger;
        private Exception m_LastException;
        private MassMessagesLogsWorker m_Logger;
        private readonly SendingFirstMessagesCache _cache;
        //-----------------------------------------------------------------------------------------
        public Exception LastException
        {
            get => m_LastException;
        }
        //-----------------------------------------------------------------------------------------
        public SendMessagePersister(IMessengerCommon _ChatMessenger, MassMessagesLogsWorker _Logger)
        {
            m_ChatMessenger = _ChatMessenger;
            m_Logger = _Logger;
            _cache = new SendingFirstMessagesCache();
        }
        //-----------------------------------------------------------------------------------------
        public bool CanTrySendFirstMessageToWhatsApp(MessengerSendAction _SendAction)
        {
            return _SendAction.MessengerSendMethod == MessengerSendMethods.FirstSendMessage
                && _SendAction.MessengerType == MessengersType.Telegram;
        }
        //-----------------------------------------------------------------------------------------
        public bool CanApplySendAction(MessengerSendAction _SendAction, out string _ErrorMessage)
        {
            _ErrorMessage = null;

            if (_SendAction.MessengerSendMethod == MessengerSendMethods.None || !_SendAction.MessengerType.HasValue)
                return false;

            if (!(_SendAction.MessengerSendMethod == MessengerSendMethods.FirstSendMessage
                && _SendAction.MessengerType == MessengersType.WhatsApp))
                return true;

            int key = GetCacheHashCodeBy(_SendAction);
            CachedSendingFirstMessage cachedMessage = _cache.Get(key);
            if (cachedMessage != null)
            {
                if (cachedMessage.ErrorMessage != null)
                    _ErrorMessage = cachedMessage.ErrorMessage;

                return cachedMessage.Result;
            }

            int httpCode;
            bool checkResponse = m_ChatMessenger.CheckPhoneNumberInSourceType(_SendAction.SourceTypeUId, _SendAction.PersonPhoneNumber, 
                out _ErrorMessage);
            if (_ErrorMessage == null)
                _ErrorMessage = "Серверная ошибка проверки существования номера телефона! " + 
                    "Попробуйте переподключить аккаунт WhatsApp в системе чат-мессенджера!";
            if (int.TryParse(_ErrorMessage, out httpCode))
            {
                switch (httpCode)
                {
                    case 200:
                        _ErrorMessage = null;
                        break;
                    case 400:
                        _ErrorMessage = $"Аккаунт в WhatsApp не поддерживается! Id аккаунта в системе мессенджеров SourceTypeUId = {_SendAction.SourceTypeUId}";
                        break;
                    case 404:
                        _ErrorMessage = $"Для номера телефона +7{_SendAction.PersonPhoneNumber} не существует WhatsApp аккаунта!";
                        break;
                }
            }

            cachedMessage = new CachedSendingFirstMessage()
            {
                Result = checkResponse,
                ErrorMessage = _ErrorMessage,
                MessengerSendMethod = _SendAction.MessengerSendMethod,
                SourceTypeUId = _SendAction.SourceTypeUId,
                PhoneNumber = _SendAction.PersonPhoneNumber,
                MessengerType = MessengersType.WhatsApp
            };
            _cache.Add(key, cachedMessage, new TimeSpan(1, 0, 0, 0));

            m_Logger.SaveMessage(new LogData()
            {
                Source = GetType().FullName,
                Message = "\r\nДействие: Произведено кэширование данных о проверке номера телефона\r\n" +
                                "Результат: " + (cachedMessage.Result ? "Успех" : "Провал") + "\r\n" +
                                "Сообщение об ошибке: " + (cachedMessage.ErrorMessage != null ? cachedMessage.ErrorMessage : "Отсутствует") + "\r\n" +
                                "Мессенджер: WhatsApp\r\n" +
                                "Тип отправки: Написать первым\r\n" +
                                $"Id WhatsApp в чат-мессенджере: {cachedMessage.SourceTypeUId}\r\n" +
                                $"Номер телефона пациента: +7{cachedMessage.PhoneNumber}"
            });

            return checkResponse;
        }
        //-----------------------------------------------------------------------------------------
        public bool ApplySendAction(MessengerSendAction _SendAction, out string _ErrorMessage)
        {
            bool result = false;

            if (!CanApplySendAction(_SendAction, out _ErrorMessage))
                return false;

            int key = GetCacheHashCodeBy(_SendAction);
            CachedSendingFirstMessage cachedMessage = _cache.Get(key);

            try
            {
                if (_SendAction.MessengerSendMethod == MessengerSendMethods.SendMessage)
                {
                    result = m_ChatMessenger.SendMessage(_SendAction.MessengerDialogUId, _SendAction.TextMessage,
                        _SendAction.Guid, _SendAction.FilePath, _SendAction.MessengerType) != null;
                }
                else if (_SendAction.MessengerSendMethod == MessengerSendMethods.FirstSendMessage)
                {
                    if (cachedMessage != null && !cachedMessage.Result)
                    {
                        result = false;
                        _ErrorMessage = cachedMessage.ErrorMessage;
                    }
                    else
                    {
                        result = m_ChatMessenger.FirstSendMessage(_SendAction.SourceTypeUId, _SendAction.PersonPhoneNumber,
                            _SendAction.TextMessage, _SendAction.Guid, _SendAction.FilePath, _SendAction.MessengerType) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                m_LastException = ex;
                _ErrorMessage = ex.Message;
            }

            if (!result && _ErrorMessage == null)
            {
                if (_SendAction.MessengerSendMethod == MessengerSendMethods.FirstSendMessage 
                    && _SendAction.MessengerType == MessengersType.Telegram)
                {
                    _ErrorMessage = "Нет возможности написать первым в Telegram-аккаунт пациента!";

                    if (cachedMessage == null)
                    {
                        cachedMessage = new CachedSendingFirstMessage()
                        {
                            Result = false,
                            ErrorMessage = _ErrorMessage,
                            MessengerSendMethod = _SendAction.MessengerSendMethod,
                            SourceTypeUId = _SendAction.SourceTypeUId,
                            PhoneNumber = _SendAction.PersonPhoneNumber,
                            MessengerType = MessengersType.Telegram
                        };
                        _cache.Add(key, cachedMessage, new TimeSpan(1, 0, 0, 0));

                        m_Logger.SaveMessage(new LogData()
                        {
                            Source = GetType().FullName,
                            Message = "\r\nДействие: Произведено кэширование данных об отправке\r\n" +
                                "Результат: " + (cachedMessage.Result ? "Успех" : "Провал") + "\r\n" +
                                "Сообщение об ошибке: " + (cachedMessage.ErrorMessage != null ? cachedMessage.ErrorMessage : "Отсутствует") + "\r\n" +
                                "Мессенджер: Telegram\r\n" +
                                "Тип отправки: Написать первым\r\n" +
                                $"Id Telegram в чат-мессенджере: {cachedMessage.SourceTypeUId}\r\n" +
                                $"Номер телефона пациента: +7{cachedMessage.PhoneNumber}"
                        });
                    }
                }
                else if (_SendAction.MessengerSendMethod == MessengerSendMethods.FirstSendMessage
                    && _SendAction.MessengerType == MessengersType.WhatsApp)
                {
                    _ErrorMessage = "Нет возможности написать первым в WhatsApp-аккаунт пациента!";
                }
                else
                {
                    _ErrorMessage = "Ошибка при отправке сообщения через чат-мессенджер!";
                }
            }

            if (_ErrorMessage != null && m_LastException != null)
                m_Logger.SaveMessage(new LogData()
                    .SetExceptionData(m_LastException)
                    .SetDeveloperComment($"Итоговое сообщение об ошибке: {_ErrorMessage}"));

            return result;
        }
        //-----------------------------------------------------------------------------------------
        private int GetCacheHashCodeBy(MessengerSendAction _SendAction)
        {
            int hashCode = _SendAction.MessengerSendMethod != MessengerSendMethods.None ? _SendAction.MessengerSendMethod.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (_SendAction.SourceTypeUId != null ? _SendAction.SourceTypeUId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (_SendAction.PersonPhoneNumber != null ? _SendAction.PersonPhoneNumber.GetHashCode() : 0);
            return hashCode;
        }
        //-----------------------------------------------------------------------------------------
    }
}
