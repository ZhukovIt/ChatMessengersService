using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    public sealed class MessengerDialogDataModel
    {
        private readonly int m_MessengerTypeId;
        private readonly string m_MessengerTypeName;
        private MessengersType? m_MessengersType;
        private bool m_IsPriority;
        private int m_MessengerDialogId;
        private string m_MessengerDialogUId;
        private int m_SourceTypeId;
        private string m_SourceTypeUId;
        private bool m_HasMessengerDialogMessagesFromPerson;
        //----------------------------------------------------------------
        public int MessengerTypeId
        {
            get => m_MessengerTypeId;
        }
        //----------------------------------------------------------------
        public string MessengerTypeName
        {
            get => m_MessengerTypeName;
        }
        //----------------------------------------------------------------
        public MessengersType? MessengersType
        {
            get => m_MessengersType;
        }
        //----------------------------------------------------------------
        public bool IsPriority
        {
            get => m_IsPriority;
            set => m_IsPriority = value;
        }
        //----------------------------------------------------------------
        public int MessengerDialogId
        {
            get => m_MessengerDialogId;
            set => m_MessengerDialogId = value;
        }
        //----------------------------------------------------------------
        public string MessengerDialogUId
        {
            get => m_MessengerDialogUId;
            set => m_MessengerDialogUId = value;
        }
        //----------------------------------------------------------------
        public int SourceTypeId
        {
            get => m_SourceTypeId;
            set => m_SourceTypeId = value;
        }
        //----------------------------------------------------------------
        public string SourceTypeUId
        {
            get => m_SourceTypeUId;
            set => m_SourceTypeUId = value;
        }
        //----------------------------------------------------------------
        public bool HasMessengerDialogMessagesFromPerson
        {
            get => m_HasMessengerDialogMessagesFromPerson;
            set => m_HasMessengerDialogMessagesFromPerson = value;
        }
        //----------------------------------------------------------------
        public MessengerDialogDataModel(int _MessengerTypeId, string _MessengerTypeName)
        {
            m_MessengerTypeId = _MessengerTypeId;
            m_MessengerTypeName = _MessengerTypeName;
            m_IsPriority = false;
            m_MessengerDialogId = -1;
            m_MessengerDialogUId = null;
            m_SourceTypeId = -1;
            m_SourceTypeUId = null;
            SetMessengersType();
        }
        //----------------------------------------------------------------
        public override int GetHashCode()
        {
            return m_MessengerTypeId.GetHashCode() ^
                m_MessengerTypeName.GetHashCode() ^
                m_HasMessengerDialogMessagesFromPerson.GetHashCode();
        }
        //----------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj is MessengerDialogDataModel other)
            {
                return m_MessengerTypeId == other.m_MessengerTypeId &&
                    m_MessengerTypeName == other.m_MessengerTypeName &&
                    m_HasMessengerDialogMessagesFromPerson == other.m_HasMessengerDialogMessagesFromPerson;
            }

            return false;
        }
        //----------------------------------------------------------------
        private void SetMessengersType()
        {
            if (m_MessengerTypeName == "WhatsApp")
            {
                m_MessengersType = SiMed.Clinic.DataModel.MessengersType.WhatsApp;
            }
            else if (m_MessengerTypeName == "Telegram")
            {
                m_MessengersType = SiMed.Clinic.DataModel.MessengersType.Telegram;
            }
            else if (m_MessengerTypeName == "Вконтакте")
            {
                m_MessengersType = SiMed.Clinic.DataModel.MessengersType.Vk;
            }
            else
            {
                m_MessengersType = null;
            }
        }
        //----------------------------------------------------------------
    }
}
