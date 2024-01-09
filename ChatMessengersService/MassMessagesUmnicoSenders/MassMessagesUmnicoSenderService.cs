using BulkMessagesWebServer;
using BulkMessagesWebServer.DataModel;
using BulkMessagesWebServer.DBObjects;
using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using BulkMessagesWebServer.DBObjects.MassMessages;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class MassMessagesUmnicoSenderService
    {
        private bool m_ThreadWorking;
        private readonly IMassMessagesRepository m_Repository;
        private readonly AbstractSenderHandler m_MainSenderHandler;
        private readonly SendMessagePersister m_SendMessagesPersister;
        private readonly ImagesPersister m_ImagesPersister;
        private readonly MassMessagesCreatorObjects m_CreatorObjects;
        private readonly MassMessagesLogsWorker m_MassMessagesLogsWorker;
        private Exception m_LastException;
        private string m_BranchName;
        //---------------------------------------------------------------
        public string BranchName
        {
            get => m_BranchName;
            set => m_BranchName = value;
        }
        //---------------------------------------------------------------
        public Exception LastException
        {
            get => m_LastException;
        }
        //---------------------------------------------------------------
        public MassMessagesUmnicoSenderService(IMessengerCommon _ChatMessenger, IMassMessagesRepository _Repository, string _BranchName)
        {
            m_Repository = _Repository;
            m_BranchName = _BranchName;
            m_MassMessagesLogsWorker = new MassMessagesLogsWorker(Application.StartupPath);
            m_MainSenderHandler = CreateMainSenderHandler();
            m_SendMessagesPersister = new SendMessagePersister(_ChatMessenger, m_MassMessagesLogsWorker);
            m_ImagesPersister = new ImagesPersister(Application.StartupPath, "MassMessagesImages");
            m_CreatorObjects = new MassMessagesCreatorObjects();
        }
        //---------------------------------------------------------------
        public void Start()
        {
            m_MassMessagesLogsWorker.SaveMessage(new LogData()
            {
                Source = GetType().FullName,
                Message = "Сервис отправки сообщений массовой рассылки успешно запущен!"
            });

            try
            {
                m_ThreadWorking = true;

                while (m_ThreadWorking)
                {
                    // Ожидаем 7 секунд, чтобы не делать частых обращений к базе данных
                    Thread.Sleep(7000);

                    // Список временных сообщений, которые будут удалены после отправки
                    List<int> _DeletedMesUmnicoSenderIds = new List<int>();

                    // Fail Fast проверка на наличие сообщений, ожидающих отправки
                    if (!m_Repository.HasSendersWhichNeedSendMessages(BranchName))
                    {
                        continue;
                    }

                    // Определяем, можно ли "Написать первым" в WhatsApp, если "Написать первым" в Телеграм завершилось неудачно
                    bool isAllowSendToWhatsApp = m_Repository.GetChatAggregatorAllowSendToWhatsAppOptions();

                    // Получаем список сообщений, которые ожидают отправки
                    IEnumerable<Sender> _SendersWhichNeedSendMessages = m_Repository.GetSendersWhichNeedSendMessages(BranchName);

                    foreach (Sender _Sender in _SendersWhichNeedSendMessages)
                    {
                        string _ErrorMessage;

                        // Получаем Id сообщения массовой рассылки
                        int? _MessageLogId = m_Repository.GetMessageLogIdByGuidAndPersonId(_Sender.Guid, _Sender.PersonId);

                        // Fail Fast  проверка, если Id ещё не появился (ожидаем его появления в сущности БД [clinic10].[dbo].[MES_LOG])
                        if (_MessageLogId == null || _MessageLogId <= 0)
                        {
                            continue;
                        }

                        // Логируем ошибку для неперсонофицированных пациентов
                        if (_Sender.PersonId <= 0)
                        {
                            _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                            _ErrorMessage = "Нельзя отправлять сообщения для неперсонофицированных пациентов!";

                            m_Repository.SetErrorInMessageLogById((int)_MessageLogId, _ErrorMessage);

                            continue;
                        }

                        // Собираем необходимые данные для выбора мессенджера
                        MessengerDialogDBContent _MessengerDialogContent = CreateMessengerDialogDBContent(_Sender.PersonId);

                        // Если сбор данных из БД завершился неудачей, тогда логируем ошибку
                        if (_MessengerDialogContent == null)
                        {
                            _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                            _ErrorMessage = m_LastException.Message;

                            m_Repository.SetErrorInMessageLogById((int)_MessageLogId, _ErrorMessage);

                            continue;
                        }

                        // Очищаем данные об ошибках от прошлой обработки сообщений
                        m_MainSenderHandler.ClearErrorData();

                        // Делегирование доменной модели принятия решения о выборе метода отправки и типа мессенджера
                        MessengerSendAction _MessengerSendAction = m_MainSenderHandler.HandleRequest(_MessengerDialogContent);

                        // Если определить тип отправки и мессенджер не получилось, логируем ошибку
                        if (!m_MainSenderHandler.CanSendMessage)
                        {
                            _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                            _ErrorMessage = m_MainSenderHandler.SendMessageError;

                            m_Repository.SetErrorInMessageLogById((int)_MessageLogId, _ErrorMessage);

                            try
                            {
                                throw new SenderHandleRequestException(_ErrorMessage);
                            }
                            catch (SenderHandleRequestException ex)
                            {
                                m_MassMessagesLogsWorker.SaveMessage(new LogData()
                                    .SetExceptionData(ex)
                                    .SetDeveloperComment(
                                        $"Ошибка произошла в доменном классе, который использует: {m_MainSenderHandler.TypeDefinition}"));

                                continue;
                            }
                        }

                        // Собираем дополнительные данные, которые необходимы для отправки сообщения
                        _MessengerSendAction.TextMessage = _Sender.Text;

                        _MessengerSendAction.Guid = _Sender.Guid;

                        if (_Sender.ImageName != null)
                        {
                            _MessengerSendAction.FilePath = m_ImagesPersister.GetFullFilePathByFileName(_Sender.ImageName);
                        }

                        // Пытаемся отправить сообщение через мессенджер
                        // Если отправка завершилась неудачей, тогда логируем ошибку
                        if (!m_SendMessagesPersister.ApplySendAction(_MessengerSendAction, out _ErrorMessage))
                        {
                            bool resultTrySend = false;
                            string oldErrorMessage = _ErrorMessage;

                            if (isAllowSendToWhatsApp && m_SendMessagesPersister.CanTrySendFirstMessageToWhatsApp(_MessengerSendAction))
                            {
                                Tuple<int, string> whatsAppData = _MessengerDialogContent.GetIntegrationData(MessengersType.WhatsApp);
                                _MessengerSendAction.SetWhatsAppAsFirstSendingVariant(whatsAppData);
                                resultTrySend = m_SendMessagesPersister.ApplySendAction(_MessengerSendAction, out _ErrorMessage);
                            }

                            if (!resultTrySend)
                            {
                                if (_ErrorMessage == null)
                                    _ErrorMessage = oldErrorMessage;

                                _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                                m_Repository.SetErrorInMessageLogById((int)_MessageLogId, _ErrorMessage);

                                continue;
                            }
                        }

                        // Собираем необходимые данные для создания объектов сообщения в базе данных
                        MessengerDialogCreateContent _MessengerDialogCreateContent =
                            _MessengerSendAction.CreateMessengerDialogCreateContent();

                        if (_Sender.ImageName != null)
                        {
                            _MessengerDialogCreateContent.SetImageAttachmentData(
                                _Sender.ImageName,
                                m_ImagesPersister.GetImageDataByFileName(_MessengerSendAction.FilePath));
                        }

                        // Добавляем объекты сообщения в базу данных
                        // В случае каких-либо проблем, логируем ошибку
                        if (!CreateMessengerDialogDBObjects(_MessengerDialogCreateContent, out _ErrorMessage))
                        {
                            _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                            m_Repository.SetErrorInMessageLogById((int)_MessageLogId, _ErrorMessage);

                            continue;
                        }

                        // Логируем успех выполнения отправки сообщения массовой рассылки
                        m_MassMessagesLogsWorker.SaveMessage(new LogData()
                        {
                            Source = GetType().FullName,
                            Message = $"Сообщение массовой рассылки с PER_ID = {_Sender.PersonId} успешно отправлено!"
                        });

                        // Добавляем сообщение массовой рассылки в список на очистку
                        _DeletedMesUmnicoSenderIds.Add(_Sender.Id);

                        // Отправка прошла успешно, поэтому отразим это в статусе сообщения массовой рассылки
                        m_Repository.UpdateMessageLogSendStateIdById((int)_MessageLogId, 2);
                    }

                    // Подчищаем ненужные временные сообщения массовой рассылки
                    foreach (int _DeletedMesUmnicoSenderId in _DeletedMesUmnicoSenderIds)
                    {
                        m_Repository.DeleteMesUmnicoSender(_DeletedMesUmnicoSenderId);
                    }
                }
            }
            catch (Exception ex)
            {
                m_LastException = ex;
                LogData errorData = new LogData()
                    .SetExceptionData(ex)
                    .SetDeveloperComment("Произошло падение общего обработчика ошибок в потоке для отправки сообщений массовой рассылки!");
                m_MassMessagesLogsWorker.SaveMessage(errorData);
            }
        }
        //---------------------------------------------------------------
        public void Stop()
        {
            m_MassMessagesLogsWorker.SaveMessage(new LogData()
            {
                Source = GetType().FullName,
                Message = "Сервис отправки сообщений массовой рассылки остановлен!"
            });

            m_ThreadWorking = false;
        }
        //---------------------------------------------------------------
        #region Вспомогательные закрытые методы
        //---------------------------------------------------------------
        private AbstractSenderHandler CreateMainSenderHandler()
        {
            AbstractSenderHandler _PriorityWhatsAppSenderHandler = new PriorityWhatsAppSenderHandler();
            AbstractSenderHandler _PriorityTelegramSenderHandler = new PriorityTelegramSenderHandler();
            AbstractSenderHandler _PriorityVkontakteSenderHandler = new PriorityVkontakteSenderHandler();
            AbstractSenderHandler _ExistsMessengerSenderHandler = new ExistsMessengerSenderHandler();
            AbstractSenderHandler _TelegramFirstSenderHandler = new TelegramFirstSenderHandler();

            _PriorityWhatsAppSenderHandler.Successor = _PriorityTelegramSenderHandler;
            _PriorityTelegramSenderHandler.Successor = _PriorityVkontakteSenderHandler;
            _PriorityVkontakteSenderHandler.Successor = _ExistsMessengerSenderHandler;
            _ExistsMessengerSenderHandler.Successor = _TelegramFirstSenderHandler;

            return _PriorityWhatsAppSenderHandler;
        }
        //---------------------------------------------------------------
        private MessengerDialogDBContent CreateMessengerDialogDBContent(int _PersonId)
        {
            MessengerDialogDBContent _Result;

            try
            {
                // Устанавливаем интеграцию с телеграм, если у пациента совсем не окажется диалогов и мессенджеров
                Tuple<int, string> _TelegramSourceTypeData = m_Repository.CreateSourceTypeData("Telegram", 1000);

                // Устанавливаем интеграцию с WhatsApp, если у пациента совсем не окажется диалогов и мессенджеров
                Tuple<int, string> _WhatsAppSourceTypeData = m_Repository.CreateSourceTypeData("WhatsApp", 1000);

                // Получаем номер телефона для пациента
                string _PersonPhoneNumber = m_Repository.GetPhoneNumberFromPersonId(_PersonId);

                // Создаём итоговый объект
                _Result = m_CreatorObjects.CreateMessengerDialogDBContent(_PersonId, _TelegramSourceTypeData, 
                    _WhatsAppSourceTypeData, _PersonPhoneNumber);

                // Поддерживаемые мессенджеры для массовой рассылки
                string[] _SupportedMessengerTypes = MassMessagesService.GetSupportedMessengerTypes();

                // Получаем все необходимые данные о мессенджерах пациента
                _Result.PersonMessengerTypes = m_Repository.GetPersonMessengerTypes(_PersonId, _SupportedMessengerTypes);

                // Получаем данные о мессенджерах, где есть диалоги с ответом пациента
                _Result.MessengerTypesWhereDialogIsExists = m_Repository.GetMessengerTypesWhereDialogIsExists(_PersonId, _Result.PersonPhoneNumber);
            }
            catch (Exception ex)
            {
                _Result = null;
                m_LastException = ex;
                LogData errorData = new LogData()
                    .SetExceptionData(ex)
                    .SetDeveloperComment("Ошибка произошла при попытке собрать данные из БД!");
                m_MassMessagesLogsWorker.SaveMessage(errorData);
            }

            return _Result;
        }
        //---------------------------------------------------------------
        private bool CreateMessengerDialogDBObjects(MessengerDialogCreateContent _Content, out string _ErrorMessage)
        {
            _ErrorMessage = null;

            try
            {
                // Определяем Id мессенджера по Id интеграции
                _Content.MessengerTypeId = m_Repository.GetMessengerTypeIdBySourceTypeId(_Content.SourceTypeId);

                // Определяем Id связки пациент-мессенджер по Id пациента и Id мессенджера
                _Content.PersonMessengerTypeId = m_Repository.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(
                    _Content.PersonId, _Content.MessengerTypeId);

                // Проверяем, что Id связки пациент-мессенджер ранее получилось найти
                // и убеждаемся, что к диалогу в мессенджере не присвоено ни одной связки пациент-мессенджер
                _Content.MessengerDialogHasNotPersonMessengerTypeId = _Content.PersonMessengerTypeId > 0 &&
                    m_Repository.PersonMessengerTypeIdInMessengerDialogIsEmpty(_Content.MessengerDialogId);

                // Делегируем доменной модели принятие решений о создании объектов базы данных
                DeciderCreateDBObjects _Decider = m_CreatorObjects.DecideToCreateDBObjects(_Content);

                if (_Decider.CanAddPersonMessengerType)
                {
                    // Добавляем новую связку пациент-мессенджер
                    m_Repository.AddPersonMessengerType(_Content.PersonId, _Content.MessengerTypeId);

                    // Обновляем Id связки пациент-мессенджер по Id пациента и Id мессенджера после создания
                    _Content.PersonMessengerTypeId = m_Repository.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(
                        _Content.PersonId, _Content.MessengerTypeId);
                }

                if (_Decider.CanAddMessengerDialog)
                {
                    // Создаём новый диалог в мессенджере
                    m_Repository.AddMessengerDialog(_Content.SourceTypeId, _Content.PhoneNumber, _Content.CreationDateTime,
                        _Content.PersonMessengerTypeId);

                    // Находим и записываем Id нового диалога в мессенджере по Id интеграции, номеру телефона и времени создания диалога
                    _Content.MessengerDialogId = m_Repository.GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(
                        _Content.SourceTypeId, _Content.PhoneNumber, _Content.CreationDateTime);
                }

                if (_Decider.CanSetPersonMessengerTypeIdByMessengerDialogId)
                {
                    // Привязываем Id связки пациент-мессенджер к диалогу в мессенджере
                    m_Repository.SetPersonMessengerTypeIdByMessengerDialogId(_Content.MessengerDialogId, _Content.PersonMessengerTypeId);
                }

                // Создаём новое сообщение для диалога в мессенджере
                m_Repository.AddMessengerDialogMessage(_Content.MessengerDialogId, _Content.MessageText, 
                    _Content.Guid, _Content.CreationDateTime);

                if (_Decider.CanAddMessengerDialogMessageAttachment)
                {
                    // Находим Id нового сообщения для диалога в мессенджере по Guid
                    int _MessengerDialogMessageId = m_Repository.GetMessengerDialogMessageIdByGuid(_Content.Guid);

                    // Создаём новое вложение для сообщения диалога в мессенджере, если есть необходимость
                    m_Repository.AddMessengerDialogMessageAttachment(_MessengerDialogMessageId, _Content.FileName, _Content.FileData);
                }
            }
            catch (Exception ex)
            {
                m_LastException = ex;
                LogData errorData = new LogData()
                    .SetExceptionData(ex)
                    .SetDeveloperComment("Ошибка произошла при попытке добавить отправленные сообщения в БД!");
                m_MassMessagesLogsWorker.SaveMessage(errorData);
                _ErrorMessage = errorData.Message;
                return false;
            }
            return true;
        }
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------
    }
}
