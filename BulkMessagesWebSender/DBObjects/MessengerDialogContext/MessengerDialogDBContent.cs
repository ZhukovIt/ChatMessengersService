using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{ 
    public sealed class MessengerDialogDBContent
    {
        private readonly int m_PersonId;
        private bool m_HasPriorityMessengerType;
        private bool m_HasMessengerDialogWithAnswerPatients;
        private IEnumerable<MessengerDialogDataModel> m_PersonMessengerTypes;
        private IEnumerable<MessengerDialogDataModel> m_MessengerTypesWhichHasInputMessages;
        private string m_PhoneNumber;
        private int m_TelegramSourceTypeId;
        private string m_TelegramSourceTypeUId;
        private int m_WhatsAppSourceTypeId;
        private string m_WhatsAppSourceTypeUId;
        //-----------------------------------------------------------------
        public int PersonId
        {
            get => m_PersonId;
        }
        //-----------------------------------------------------------------
        public string PersonPhoneNumber
        {
            get => m_PhoneNumber;
            set => m_PhoneNumber = value;
        }
        //-----------------------------------------------------------------
        public int TelegramSourceTypeId
        {
            get => m_TelegramSourceTypeId;
            set => m_TelegramSourceTypeId = value;
        }
        //-----------------------------------------------------------------
        public string TelegramSourceTypeUId
        {
            get => m_TelegramSourceTypeUId;
            set => m_TelegramSourceTypeUId = value;
        }
        //-----------------------------------------------------------------
        public int WhatsAppSourceTypeId
        {
            get => m_WhatsAppSourceTypeId;
            set => m_WhatsAppSourceTypeId = value;
        }
        //-----------------------------------------------------------------
        public string WhatsAppSourceTypeUId
        {
            get => m_WhatsAppSourceTypeUId;
            set => m_WhatsAppSourceTypeUId = value;
        }
        //-----------------------------------------------------------------
        public bool HasPriorityMessengerType
        {
            get => m_HasPriorityMessengerType;
        }
        //-----------------------------------------------------------------
        public bool HasMessengerDialogWithAnswerPatients
        {
            get => m_HasMessengerDialogWithAnswerPatients;
        }
        //-----------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> PersonMessengerTypes
        {
            get => m_PersonMessengerTypes;
            
            set
            {
                m_PersonMessengerTypes = value;

                m_HasPriorityMessengerType = value.Any(mt => mt.IsPriority);
            }
        }
        //-----------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> MessengerTypesWhereDialogIsExists
        {
            get => m_MessengerTypesWhichHasInputMessages;
            
            set
            {
                m_MessengerTypesWhichHasInputMessages = value;

                m_HasMessengerDialogWithAnswerPatients = value.Any();
            }
        }
        //-----------------------------------------------------------------
        public MessengerDialogDBContent(int _PersonId)
        {
            m_PersonId = _PersonId;
            m_HasPriorityMessengerType = false;
            m_PhoneNumber = null;
            m_TelegramSourceTypeUId = null;
            m_WhatsAppSourceTypeUId = null;
        }
        //-----------------------------------------------------------------
        public Tuple<int, string> GetIntegrationData(MessengersType _MessengerType)
        {
            switch (_MessengerType)
            {
                case MessengersType.WhatsApp:
                    return Tuple.Create(m_WhatsAppSourceTypeId, m_WhatsAppSourceTypeUId);
                case MessengersType.Telegram:
                    return Tuple.Create(m_TelegramSourceTypeId, m_TelegramSourceTypeUId);
                default:
                    throw new NotImplementedException($"Тип мессенджера MessengerType = {_MessengerType} не поддерживается!");
            }
        }
        //-----------------------------------------------------------------
    }
}
