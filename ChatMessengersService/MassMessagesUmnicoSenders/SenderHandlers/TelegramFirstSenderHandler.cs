using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiMed.Clinic.DataModel;
using BulkMessagesWebServer;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class TelegramFirstSenderHandler : AbstractSenderHandler
    {
        protected override string Definition => "Telegram для написания первого сообщения пациенту";
        //------------------------------------------------------------------
        public override MessengerSendAction HandleRequest(MessengerDialogDBContent _Content)
        {
            bool _Condition = MassMessagesService.GetCorrectPhoneNumber(_Content.PersonPhoneNumber) != null;
            if (!_Condition)
                SendMessageError = "У пациента нет подходящих диалогов в чат-мессенджере и отсутствует номер телефона!";

            MessengerDialogDataModel _MessengerDialogDataModel = null;
            string _SourceTypeUId = null;
            int _SourceTypeId = -1;

            if (_Condition)
            {
                _MessengerDialogDataModel = _Content.PersonMessengerTypes
                    .FirstOrDefault(md => md.MessengersType == MessengersType.Telegram);

                if (_MessengerDialogDataModel != null)
                {
                    _SourceTypeId = _MessengerDialogDataModel.SourceTypeId;
                    _SourceTypeUId = _MessengerDialogDataModel.SourceTypeUId;
                }
                else if (_Content.TelegramSourceTypeUId == null)
                {
                    SendMessageError = "Не нашлось ни одного аккаунта Telegram для отправки первого сообщения!";
                }
                else if (_Content.TelegramSourceTypeUId != null)
                {
                    _SourceTypeId = _Content.TelegramSourceTypeId;
                    _SourceTypeUId = _Content.TelegramSourceTypeUId;
                }

                _Condition &= _Content.TelegramSourceTypeUId != null;
            }

            if (_Condition)
            {
                return new MessengerSendAction(MessengerSendMethods.FirstSendMessage, MessengersType.Telegram)
                    .SetPersonId(_Content.PersonId)
                    .SetSourceTypeId(_SourceTypeId)
                    .SetSourceTypeUId(_SourceTypeUId)
                    .SetPersonPhoneNumber(_Content.PersonPhoneNumber);
            }
            else if (m_Successor != null)
            {
                return m_Successor.HandleRequest(_Content);
            }
            else if (SendMessageError == null)
            {
                SendMessageError = "Отправка первого сообщения от аккаунта Telegram невозможна!";
            }

            return new MessengerSendAction(MessengerSendMethods.None, null);
        }
        //------------------------------------------------------------------
    }
}
