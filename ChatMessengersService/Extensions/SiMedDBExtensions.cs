using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAggregatorMessage = SiMed.Clinic.DataModel.Message;

namespace ChatMessengersService
{
    public static class SiMedDBExtensions
    {
        public static dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow Clone(this dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow _Prototype,
            dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable _DataTable)
        {
            var _NewMessengerDialogMessageRow = _DataTable.NewMESSENGER_DIALOG_MESSAGERow();

            _NewMessengerDialogMessageRow.MES_DIAL_MES_DEPARTURE_DATE = _Prototype.MES_DIAL_MES_DEPARTURE_DATE;

            _NewMessengerDialogMessageRow.MES_TYPE_ID = _Prototype.MES_TYPE_ID;

            _NewMessengerDialogMessageRow.MES_DIAL_ID = _Prototype.MES_DIAL_ID;

            _NewMessengerDialogMessageRow.MES_STAT_TYPE_ID = _Prototype.MES_STAT_TYPE_ID;

            if (!_Prototype.IsMES_DIAL_MES_EXTERNAL_IDNull())
            {
                _NewMessengerDialogMessageRow.MES_DIAL_MES_EXTERNAL_ID = _Prototype.MES_DIAL_MES_EXTERNAL_ID;
            }

            if (!_Prototype.IsMES_DIAL_MES_URLNull())
            {
                _NewMessengerDialogMessageRow.MES_DIAL_MES_URL = _Prototype.MES_DIAL_MES_URL;
            }

            return _NewMessengerDialogMessageRow;
        }
        //-----------------------------------------------------
        public static IEnumerable<ChatAggregatorMessage> FilterOutWantedMessages(this IEnumerable<ChatAggregatorMessage> _Messages, 
            IEnumerable<string> _MessengerDialogUIdsForBlockingMessengerDialogs)
        {
            List<ChatAggregatorMessage> _FilteredMessages = new List<ChatAggregatorMessage>();

            foreach (ChatAggregatorMessage _Message in _Messages)
            {
                string _ChatDialogId = _Message.ChatDialog.Id;

                if (!_MessengerDialogUIdsForBlockingMessengerDialogs.Contains(_ChatDialogId))
                {
                    _FilteredMessages.Add(_Message);
                }
            }

            return _FilteredMessages;
        }
        //-----------------------------------------------------
    }
}
