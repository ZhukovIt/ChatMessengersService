using BulkMessagesWebServer.DBObjects.MassMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;

namespace BulkMessagesWebServer.DBObjects
{
    public interface IMassMessagesRepository
    {
        void AddNewMesUmnicoSender(int _PersonId, string _Text, string _Guid, string _ImageName, string _BranchName);

        void DeleteMesUmnicoSender(int _MesUmnSendId);

        IEnumerable<Sender> GetSendersWhichNeedSendMessages(string _BranchName);

        bool HasSendersWhichNeedSendMessages(string _BranchName);

        Tuple<int, string> CreateSourceTypeData(string _MessengerTypeName, int _StartIDToFind = 1);

        string GetPhoneNumberFromPersonId(int _PersonId);

        IEnumerable<MessengerDialogDataModel> GetPersonMessengerTypes(int _PersonId, IEnumerable<string> _SupportedMessengerTypes);

        IEnumerable<MessengerDialogDataModel> GetMessengerTypesWhereDialogIsExists(int _PersonId, string _PersonPhoneNumber);

        int GetMessengerTypeIdBySourceTypeId(int _SourceTypeId);

        int GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(int _PersonId, int _MessengerTypeId);

        bool PersonMessengerTypeIdInMessengerDialogIsEmpty(int _MessengerDialogId);

        void AddPersonMessengerType(int _PersonId, int _MessengerTypeId);

        void AddMessengerDialog(int _SourceTypeId, string _PhoneNumber, DateTime _CreationDateTime, int? _PersonMessengerTypeId = null);

        int GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(int _SourceTypeId, string _PhoneNumber, DateTime _CreationDateTime);

        void SetPersonMessengerTypeIdByMessengerDialogId(int _MessengerDialogId, int _PersonMessengerTypeId);

        void AddMessengerDialogMessage(int _MessengerDialogId, string _Text, string _Guid, DateTime _CreationDateTime);

        int GetMessengerDialogMessageIdByGuid(string _Guid);

        void AddMessengerDialogMessageAttachment(int _MessengerDialogMessageId, string _FileName, string _Data);

        int? GetMessageLogIdByGuidAndPersonId(string _Guid, int _PersonId);

        void UpdateMessageLogSendStateIdById(int _MesssageLogId, int _NewMessageLogSendStateId);

        void SetErrorInMessageLogById(int _MessageLogId, string _ErrorMessage);

        bool GetChatAggregatorAllowSendToWhatsAppOptions();
    }
}
