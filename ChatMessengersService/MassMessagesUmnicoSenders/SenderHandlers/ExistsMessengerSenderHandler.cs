using BulkMessagesWebServer.DBObjects.MessengerDialog;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class ExistsMessengerSenderHandler : AbstractSenderHandler
    {
        protected override string Definition => "Имеется какой-либо диалог с пациентом";
        //------------------------------------------------------------------
        public override MessengerSendAction HandleRequest(MessengerDialogDBContent _Content)
        {
            bool _Condition = _Content.HasMessengerDialogWithAnswerPatients;

            MessengerDialogDataModel _MessengerDialogDataModel = null;

            if (_Condition)
            {
                _MessengerDialogDataModel = _Content.MessengerTypesWhereDialogIsExists
                    .FirstOrDefault(md => md.MessengersType == MessengersType.Telegram);

                if (_MessengerDialogDataModel == null)
                {
                    _MessengerDialogDataModel = _Content.MessengerTypesWhereDialogIsExists
                        .FirstOrDefault(md => md.MessengersType == MessengersType.WhatsApp &&
                            md.HasMessengerDialogMessagesFromPerson);

                    if (_MessengerDialogDataModel == null)
                    {
                        _MessengerDialogDataModel = _Content.MessengerTypesWhereDialogIsExists
                            .FirstOrDefault(md => md.MessengersType == MessengersType.Vk);

                        if (_MessengerDialogDataModel == null)
                        {
                            _Condition = false;
                        }
                    }
                }
            }

            if (_Condition)
            {
                return new MessengerSendAction(MessengerSendMethods.SendMessage,
                    (MessengersType)_MessengerDialogDataModel.MessengersType)
                    .SetPersonId(_Content.PersonId)
                    .SetSourceTypeId(_MessengerDialogDataModel.SourceTypeId)
                    .SetMessengerDialogId(_MessengerDialogDataModel.MessengerDialogId)
                    .SetMessengerDialogUId(_MessengerDialogDataModel.MessengerDialogUId);
            }
            else if (m_Successor != null)
            {
                return m_Successor.HandleRequest(_Content);
            }

            SendMessageError = "У пациента не нашлось диалогов, которые подошли бы для отправки в чат-мессенджере!";
            return new MessengerSendAction(MessengerSendMethods.None, null);
        }
    }
}
