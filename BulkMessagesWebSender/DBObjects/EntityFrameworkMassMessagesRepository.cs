using BulkMessagesWebServer.DBObjects.MassMessages;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using MessengerDialogPerson = BulkMessagesWebServer.DBObjects.MessengerDialog.Person;
using BulkMessagesWebServer.DBObjects.MassMessagesDBLogs;
using BulkMessagesWebServer.DBObjects.OptionsContext;

namespace BulkMessagesWebServer.DBObjects
{
    public sealed class EntityFrameworkMassMessagesRepository : IMassMessagesRepository
    {
        private readonly string m_NameConnectionString;
        //----------------------------------------------------------------------------------
        public EntityFrameworkMassMessagesRepository(string _NameConnectionString)
        {
            m_NameConnectionString = _NameConnectionString;
        }
        //----------------------------------------------------------------------------------
        public void AddNewMesUmnicoSender(int _PersonId, string _Text, string _Guid, string _ImageName, string _BranchName)
        {
            using (MassMessagesContext db = new MassMessagesContext(m_NameConnectionString))
            {
                Sender _NewSender = new Sender
                {
                    SenderStatusTypeId = 1,
                    Guid = _Guid,
                    Text = _Text,
                    PersonId = _PersonId,
                    ImageName = _ImageName,
                    BranchName = _BranchName
                };

                db.Senders.Add(_NewSender);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------------------------------
        public void DeleteMesUmnicoSender(int _MesUmnSendId)
        {
            using (MassMessagesContext db = new MassMessagesContext(m_NameConnectionString))
            {
                Sender _DeletedSender = db.Senders.AsQueryable().FirstOrDefault(row => row.Id == _MesUmnSendId);

                if (_DeletedSender != null)
                {
                    db.Senders.Remove(_DeletedSender);

                    db.SaveChanges();
                }
            }
        }
        //----------------------------------------------------------------------------------
        public IEnumerable<Sender> GetSendersWhichNeedSendMessages(string _BranchName)
        {
            using (MassMessagesContext db = new MassMessagesContext(m_NameConnectionString))
            {
                return db.Senders
                    .AsQueryable()
                    .Where(s => s.BranchName == _BranchName && s.SenderStatusTypeId == 1)
                    .ToList();
            }
        }
        //----------------------------------------------------------------------------------
        public bool HasSendersWhichNeedSendMessages(string _BranchName)
        {
            using (MassMessagesContext db = new MassMessagesContext(m_NameConnectionString))
            {
                return db.Senders
                    .AsQueryable()
                    .Where(s => s.BranchName == _BranchName && s.SenderStatusTypeId == 1)
                    .Count() > 0;
            }
        }
        //----------------------------------------------------------------------------------
        public Tuple<int, string> CreateSourceTypeData(string _MessengerTypeName, int _StartIDToFind = 1)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                return (from st in db.SourceTypes
                        join mt in db.MessengerTypes on st.MessengerTypeId equals mt.Id
                        where mt.Name == _MessengerTypeName && st.Id >= _StartIDToFind
                        select new
                        {
                            SourceTypeId = st.Id,
                            st.SourceTypeUId
                        })
                     .AsEnumerable()
                     .Select(x => Tuple.Create(x.SourceTypeId, x.SourceTypeUId))
                     .FirstOrDefault();
            }
        }
        //----------------------------------------------------------------------------------
        public string GetPhoneNumberFromPersonId(int _PersonId)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                return (from a in db.Addresses
                        join p in db.Persons on a.Id equals p.PersonLiveAddress
                        where p.Id == _PersonId &&
                           a.FirstPhone != null &&
                           a.FirstPhone.Length >= 10
                        select a.FirstPhone)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
        }
        //----------------------------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> GetPersonMessengerTypes(int _PersonId, IEnumerable<string> _SupportedMessengerTypes)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                return (from p in db.Persons
                        join pmt in db.PersonMessengerTypes
                            on p.Id equals pmt.PersonId
                        join mt in db.MessengerTypes
                            on pmt.MessengerTypeId equals mt.Id
                        join st in db.SourceTypes
                            on mt.Id equals st.MessengerTypeId
                        where p.Id == _PersonId && _SupportedMessengerTypes.Contains(mt.Name)
                        select new
                        {
                            MessengerTypeId = mt.Id,
                            MessengerTypeName = mt.Name,
                            IsPriority = mt.Id == p.PriorityMessengerTypeId,
                            SourceTypeId = st.Id,
                            st.SourceTypeUId
                        })
                     .AsEnumerable()
                     .Select(x => new MessengerDialogDataModel(x.MessengerTypeId, x.MessengerTypeName)
                     {
                         IsPriority = x.IsPriority,
                         SourceTypeId = x.SourceTypeId,
                         SourceTypeUId = x.SourceTypeUId
                     })
                     .ToList();
            }
        }
        //----------------------------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> GetMessengerTypesWhereDialogIsExists(int _PersonId, string _PersonPhoneNumber)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                return (from mt in db.MessengerTypes
                        join st in db.SourceTypes
                           on mt.Id equals st.MessengerTypeId
                        join pmt in db.PersonMessengerTypes
                            on mt.Id equals pmt.MessengerTypeId
                        join p in db.Persons
                            on pmt.PersonId equals p.Id
                        join md in db.MessengerDialogs
                            on pmt.Id equals md.PersonMessengerTypeId
                        where p.Id == _PersonId &&
                            md.MessengerDialogTypeId == 1 &&
                            md.MessengerDialogUId != null
                        select new
                        {
                            MessengerTypeId = mt.Id,
                            MessengerTypeName = mt.Name,
                            MessengerDialogId = md.Id,
                            md.MessengerDialogUId,
                            SourceTypeId = st.Id,
                            HasMessengerDialogMessagesFromPerson = (
                                from mdm2 in db.MessengerDialogMessages
                                where mdm2.MessengerDialogId == md.Id &&
                                    mdm2.MessengerDialogMessageTypeId == 1
                                select mdm2).Count() > 0
                        })
                     .Union(
                        from md in db.MessengerDialogs
                        join st in db.SourceTypes
                            on md.SourceTypeId equals st.Id
                        join mt in db.MessengerTypes
                            on st.MessengerTypeId equals mt.Id
                        where _PersonPhoneNumber != null &&
                            (md.Login.Contains(_PersonPhoneNumber) ||
                            md.Phone.Contains(_PersonPhoneNumber)) &&
                            md.MessengerDialogTypeId == 1 &&
                            md.MessengerDialogUId != null &&
                            md.PersonMessengerTypeId == null
                        select new
                        {
                            MessengerTypeId = mt.Id,
                            MessengerTypeName = mt.Name,
                            MessengerDialogId = md.Id,
                            md.MessengerDialogUId,
                            SourceTypeId = st.Id,
                            HasMessengerDialogMessagesFromPerson = (
                            from mdm2 in db.MessengerDialogMessages
                            where mdm2.MessengerDialogId == md.Id &&
                                mdm2.MessengerDialogMessageTypeId == 1
                            select mdm2
                            )
                            .Count() > 0
                        }
                    )
                    .Distinct()
                    .AsEnumerable()
                    .Select(x => new MessengerDialogDataModel(x.MessengerTypeId, x.MessengerTypeName)
                    {
                        MessengerDialogId = x.MessengerDialogId,
                        MessengerDialogUId = x.MessengerDialogUId,
                        SourceTypeId = x.SourceTypeId,
                        HasMessengerDialogMessagesFromPerson = x.HasMessengerDialogMessagesFromPerson
                    })
                    .Distinct()
                    .ToList();
            }
        }
        //----------------------------------------------------------------------------------
        public int GetMessengerTypeIdBySourceTypeId(int _SourceTypeId)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                SourceType _SourceType = db.SourceTypes.AsQueryable()
                    .First(st => st.Id == _SourceTypeId);

                return _SourceType.MessengerTypeId;
            }
        }
        //----------------------------------------------------------------------------------
        public int GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(int _PersonId, int _MessengerTypeId)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                PersonMessengerType _PersonMessengerType = db.PersonMessengerTypes.AsQueryable()
                    .FirstOrDefault(pmt => pmt.PersonId == _PersonId && pmt.MessengerTypeId == _MessengerTypeId);

                if (_PersonMessengerType != null)
                {
                    return _PersonMessengerType.Id;
                }
            }

            return -1;
        }
        //----------------------------------------------------------------------------------
        public bool PersonMessengerTypeIdInMessengerDialogIsEmpty(int _MessengerDialogId)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialog _MessengerDialog = db.CreationMessengerDialogs.AsQueryable()
                    .First(md => md.Id == _MessengerDialogId);

                return _MessengerDialog.PersonMessengerTypeId == null;
            }
        }
        //----------------------------------------------------------------------------------
        public void AddPersonMessengerType(int _PersonId, int _MessengerTypeId)
        {
            using (MessengerDialogContext db = new MessengerDialogContext(m_NameConnectionString))
            {
                PersonMessengerType _NewPersonMessengerType = new PersonMessengerType()
                {
                    MessengerTypeId = _MessengerTypeId,
                    PersonId = _PersonId
                };

                db.PersonMessengerTypes.Add(_NewPersonMessengerType);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------------------------------
        public void AddMessengerDialog(int _SourceTypeId, string _PhoneNumber, DateTime _CreationDateTime, int? _PersonMessengerTypeId = null)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialog _NewMessengerDialog = new CreationMessengerDialog
                {
                    SourceTypeId = _SourceTypeId,
                    TypeId = 1,
                    StatusTypeId = 1,
                    CreationDateTime = _CreationDateTime,
                    Login = _PhoneNumber,
                    Phone = _PhoneNumber,
                    IsRead = 0,
                    PersonMessengerTypeId = _PersonMessengerTypeId
                };

                db.CreationMessengerDialogs.Add(_NewMessengerDialog);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------------------------------
        public int GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(int _SourceTypeId, string _PhoneNumber, 
            DateTime _CreationDateTime)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialog _MessengerDialog = db.CreationMessengerDialogs.AsQueryable()
                    .Where(md => md.SourceTypeId == _SourceTypeId &&
                        (md.Login == _PhoneNumber || md.Phone == _PhoneNumber))
                    .AsEnumerable()
                    .FirstOrDefault(md => md.CreationDateTime.DateTimeEquals(_CreationDateTime));

                if (_MessengerDialog != null)
                {
                    return _MessengerDialog.Id;
                }

                return -1;
            }
        }
        //----------------------------------------------------------------------------------
        public void SetPersonMessengerTypeIdByMessengerDialogId(int _MessengerDialogId, int _PersonMessengerTypeId)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialog _MessengerDialog = db.CreationMessengerDialogs.AsQueryable()
                    .FirstOrDefault(md => md.Id == _MessengerDialogId);

                if (_MessengerDialog != null)
                {
                    _MessengerDialog.PersonMessengerTypeId = _PersonMessengerTypeId;
                    db.SaveChanges();
                }
            }
        }
        //----------------------------------------------------------------------------------
        public void AddMessengerDialogMessage(int _MessengerDialogId, string _Text, string _Guid, DateTime _CreationDateTime)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialogMessage _NewMessengerDialogMessage = new CreationMessengerDialogMessage
                {
                    TypeId = 2,
                    StatusTypeId = 2,
                    MessengerDialogId = _MessengerDialogId,
                    Text = _Text,
                    CreationDateTime = _CreationDateTime,
                    Guid = _Guid,
                    AuthorName = "Менеджер массовой рассылки"
                };

                db.CreationMessengerDialogMessages.Add(_NewMessengerDialogMessage);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------------------------------
        public int GetMessengerDialogMessageIdByGuid(string _Guid)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialogMessage _MessengerDialogMessage = db.CreationMessengerDialogMessages
                    .AsQueryable().FirstOrDefault(mdm => mdm.Guid == _Guid);

                if (_MessengerDialogMessage != null)
                {
                    return _MessengerDialogMessage.Id;
                }

                return -1;
            }
        }
        //----------------------------------------------------------------------------------
        public void AddMessengerDialogMessageAttachment(int _MessengerDialogMessageId, string _FileName, string _Data)
        {
            using (CreateMessengerDialogContext db = new CreateMessengerDialogContext(m_NameConnectionString))
            {
                CreationMessengerDialogMessageAttachment _NewMessengerDialogMessageAttachment =
                    new CreationMessengerDialogMessageAttachment
                    {
                        MessengerDialogMessageId = _MessengerDialogMessageId,
                        Name = _FileName,
                        Data = _Data
                    };

                db.CreationMessengerDialogMessageAttachments.Add(_NewMessengerDialogMessageAttachment);
                db.SaveChanges();
            }
        }
        //----------------------------------------------------------------------------------
        public int? GetMessageLogIdByGuidAndPersonId(string _Guid, int _PersonId)
        {
            using (MassMessagesDBLogsContext db = new MassMessagesDBLogsContext(m_NameConnectionString))
            {
                MessageLog _MessageLog;

                MessageBatch _MessageBatch = db.MessageBatches.AsQueryable()
                    .FirstOrDefault(mb => mb.ExternalId == _Guid);

                if (_MessageBatch != null)
                {
                    _MessageLog = db.MessageLogs.AsQueryable()
                        .FirstOrDefault(ml => ml.MessageBatchId == _MessageBatch.Id &&
                            ml.PersonId == _PersonId);
                }
                else
                {
                    _MessageLog = db.MessageLogs.AsQueryable()
                        .FirstOrDefault(ml => ml.ExternalId == _Guid);
                }

                return _MessageLog?.Id;
            }
        }
        //----------------------------------------------------------------------------------
        public void UpdateMessageLogSendStateIdById(int _MessageLogId, int _NewMessageLogSendStateId)
        {
            using (MassMessagesDBLogsContext db = new MassMessagesDBLogsContext(m_NameConnectionString))
            {
                MessageLog _MessageLog = db.MessageLogs.AsQueryable()
                    .FirstOrDefault(ml => ml.Id == _MessageLogId);

                if (_MessageLog != null)
                {
                    _MessageLog.SendStatusId = _NewMessageLogSendStateId;

                    db.SaveChanges();
                }
            }
        }
        //----------------------------------------------------------------------------------
        public void SetErrorInMessageLogById(int _MessageLogId, string _ErrorMessage)
        {
            using (MassMessagesDBLogsContext db = new MassMessagesDBLogsContext(m_NameConnectionString))
            {
                MessageLog _MessageLog = db.MessageLogs.AsQueryable()
                    .FirstOrDefault(ml => ml.Id == _MessageLogId);

                if (_MessageLog != null)
                {
                    _MessageLog.SendStatusId = 3;

                    _MessageLog.SendErrorMessage = _ErrorMessage;

                    db.SaveChanges();
                }
            }
        }
        //----------------------------------------------------------------------------------
        public bool GetChatAggregatorAllowSendToWhatsAppOptions()
        {
            using (OptionsContext.OptionsContext db = new OptionsContext.OptionsContext(m_NameConnectionString))
            {
                Option option = db.Options.AsQueryable()
                    .FirstOrDefault(opt => opt.Name.Contains("ChatAggregatorAllowSendToWhatsApp"));
                if (option == null)
                    return false;

                return byte.Parse(option.Value) > 0;
            }
        }
        //----------------------------------------------------------------------------------
    }
}
