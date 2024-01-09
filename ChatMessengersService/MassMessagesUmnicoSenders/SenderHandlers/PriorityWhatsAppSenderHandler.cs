using BulkMessagesWebServer.DBObjects.MessengerDialog;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class PriorityWhatsAppSenderHandler : AbstractSenderHandler
    {
        protected override string Definition => "WhatsApp приоритетный мессенджер и имеется диалог с ответами от пациента";
        //------------------------------------------------------------------
        public override MessengerSendAction HandleRequest(MessengerDialogDBContent _Content)
        {
            bool _Condition = _Content.HasPriorityMessengerType;

            if (_Condition)
            {
                _Condition &= _Content.PersonMessengerTypes
                    .First(mt => mt.IsPriority)
                    .MessengersType == MessengersType.WhatsApp;
            }

            MessengerDialogDataModel _MessengerDialogDataModel = null;

            if (_Condition)
            {
                _MessengerDialogDataModel = _Content.MessengerTypesWhereDialogIsExists
                    .FirstOrDefault(
                        md => md.MessengersType == MessengersType.WhatsApp &&
                            md.HasMessengerDialogMessagesFromPerson);

                _Condition &= _MessengerDialogDataModel != null;
            }

            if (_Condition)
            {
                return new MessengerSendAction(MessengerSendMethods.SendMessage, MessengersType.WhatsApp)
                    .SetPersonId(_Content.PersonId)
                    .SetSourceTypeId(_MessengerDialogDataModel.SourceTypeId)
                    .SetMessengerDialogId(_MessengerDialogDataModel.MessengerDialogId)
                    .SetMessengerDialogUId(_MessengerDialogDataModel.MessengerDialogUId);
            }
            else if (m_Successor != null)
            {
                return m_Successor.HandleRequest(_Content);
            }

            SendMessageError = "Отправка из WhatsApp (приоритетный мессенджер) с учётом наличия ответа от пациента невозможна!";
            return new MessengerSendAction(MessengerSendMethods.None, null);
        }
    }
}
