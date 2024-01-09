using BulkMessagesWebServer;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class PriorityTelegramSenderHandler : AbstractSenderHandler
    {
        protected override string Definition => "Telegram приоритетный мессенджер";
        //------------------------------------------------------------------
        public override MessengerSendAction HandleRequest(MessengerDialogDBContent _Content)
        {
            MessengerSendAction _Result = null;

            bool _Condition = _Content.HasPriorityMessengerType;

            MessengerDialogDataModel _MessengerDialogDataModelForFirstSend = null;
            MessengerDialogDataModel _MessengerDialogDataModelForSend = null;

            if (_Condition)
            {
                _MessengerDialogDataModelForFirstSend = _Content.PersonMessengerTypes
                    .FirstOrDefault(md => md.IsPriority && md.MessengersType == MessengersType.Telegram);

                _Condition &= _MessengerDialogDataModelForFirstSend != null;
            }

            MessengerSendMethods _MessengerSendMethod = MessengerSendMethods.None;

            if (_Condition)
            {
                _MessengerDialogDataModelForSend = _Content.MessengerTypesWhereDialogIsExists
                    .FirstOrDefault(mt => mt.MessengersType == MessengersType.Telegram);

                _MessengerSendMethod = _MessengerDialogDataModelForSend != null ?
                    MessengerSendMethods.SendMessage : MessengerSendMethods.FirstSendMessage;

                if (_MessengerSendMethod == MessengerSendMethods.SendMessage)
                {
                    _Result = new MessengerSendAction(_MessengerSendMethod, MessengersType.Telegram)
                        .SetPersonId(_Content.PersonId)
                        .SetSourceTypeId(_MessengerDialogDataModelForSend.SourceTypeId)
                        .SetMessengerDialogId(_MessengerDialogDataModelForSend.MessengerDialogId)
                        .SetMessengerDialogUId(_MessengerDialogDataModelForSend.MessengerDialogUId);
                }
                else if (_MessengerSendMethod == MessengerSendMethods.FirstSendMessage)
                {
                    _Condition &= MassMessagesService.GetCorrectPhoneNumber(_Content.PersonPhoneNumber) != null;

                    if (_Content.PersonPhoneNumber == null)
                    {
                        SendMessageError = "У пациента отсутствует номер телефона!";
                    }

                    if (_Condition)
                    {
                        _Result = new MessengerSendAction(_MessengerSendMethod, MessengersType.Telegram)
                            .SetPersonId(_Content.PersonId)
                            .SetSourceTypeId(_MessengerDialogDataModelForFirstSend.SourceTypeId)
                            .SetSourceTypeUId(_MessengerDialogDataModelForFirstSend.SourceTypeUId)
                            .SetPersonPhoneNumber(_Content.PersonPhoneNumber);
                    }
                }
            }

            if (_Condition)
            {
                SendMessageError = null;
                return _Result;
            }
            else if (m_Successor != null)
            {
                return m_Successor.HandleRequest(_Content);
            }

            if (SendMessageError == null)
            {
                SendMessageError = "Отправка из Telegram (приоритетный мессенджер) невозможна!";
            }
            
            return new MessengerSendAction(MessengerSendMethods.None, null);
        }
    }
}
