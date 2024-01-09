using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using SiMed.Clinic.DataModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class PriorityVkontakteSenderHandler : AbstractSenderHandler
    {
        protected override string Definition => "ВКонтакте приоритетный мессенджер и имеется диалог с пациентом";
        //------------------------------------------------------------------
        public override MessengerSendAction HandleRequest(MessengerDialogDBContent _Content)
        {
            bool _Condition = _Content.HasPriorityMessengerType;

            if (_Condition)
            {
                _Condition &= _Content.PersonMessengerTypes
                    .First(mt => mt.IsPriority)
                    .MessengersType == MessengersType.Vk;
            }

            MessengerDialogDataModel _MessengerDialogDataModel = null;

            if (_Condition)
            {
                _MessengerDialogDataModel = _Content.MessengerTypesWhereDialogIsExists
                    .FirstOrDefault(
                        md => md.MessengersType == MessengersType.Vk);

                _Condition &= _MessengerDialogDataModel != null;
            }

            if (_Condition)
            {
                return new MessengerSendAction(MessengerSendMethods.SendMessage, MessengersType.Vk)
                    .SetPersonId(_Content.PersonId)
                    .SetSourceTypeId(_MessengerDialogDataModel.SourceTypeId)
                    .SetMessengerDialogId(_MessengerDialogDataModel.MessengerDialogId)
                    .SetMessengerDialogUId(_MessengerDialogDataModel.MessengerDialogUId);
            }
            else if (m_Successor != null)
            {
                return m_Successor.HandleRequest(_Content);
            }

            SendMessageError = "Отправка из ВКонтакте (приоритетный мессенджер) невозможна!";
            return new MessengerSendAction(MessengerSendMethods.None, null);
        }
    }
}
