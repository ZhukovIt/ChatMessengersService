using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using SiMed.Clinic.DataModel;
using System;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public sealed class MessengerSendAction
    {
        private MessengerSendMethods m_MessengerSendMethod;
        private MessengersType? m_MessengerType;
        private int m_PersonId;
        private int m_MessengerDialogId;
        private string m_MessengerDialogUId;
        private int m_SourceTypeId;
        private string m_SourceTypeUId;
        private string m_PersonPhoneNumber;
        private string m_TextMessage;
        private string m_Guid;
        private string m_FilePath;
        //--------------------------------------------------------------------------
        public string FilePath
        {
            get => m_FilePath;
            set => m_FilePath = value;
        }
        //--------------------------------------------------------------------------
        public string Guid
        {
            get => m_Guid;
            set => m_Guid = value;
        }
        //--------------------------------------------------------------------------
        public string TextMessage
        {
            get => m_TextMessage;
            set => m_TextMessage = value;
        }
        //--------------------------------------------------------------------------
        public int PersonId
        {
            get => m_PersonId;
        }
        //--------------------------------------------------------------------------
        public int MessengerDialogId
        {
            get => m_MessengerDialogId;
        }
        //--------------------------------------------------------------------------
        public string MessengerDialogUId
        {
            get => m_MessengerDialogUId;
        }
        //--------------------------------------------------------------------------
        public int SourceTypeId
        {
            get => m_SourceTypeId;
        }
        //--------------------------------------------------------------------------
        public string SourceTypeUId
        {
            get => m_SourceTypeUId;
        }
        //--------------------------------------------------------------------------
        public string PersonPhoneNumber
        {
            get => m_PersonPhoneNumber;
        }
        //--------------------------------------------------------------------------
        public MessengerSendMethods MessengerSendMethod
        {
            get => m_MessengerSendMethod;
        }
        //--------------------------------------------------------------------------
        public MessengersType? MessengerType
        {
            get => m_MessengerType;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction(MessengerSendMethods _MessengerSendMethod, MessengersType? _MessengerType)
        {
            m_MessengerSendMethod = _MessengerSendMethod;
            m_MessengerType = _MessengerType;
            m_MessengerDialogId = -1;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetMessengerDialogId(int _MessengerDialogId)
        {
            m_MessengerDialogId = _MessengerDialogId;
            return this;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetMessengerDialogUId(string _MessengerDialogUId)
        {
            m_MessengerDialogUId = _MessengerDialogUId;
            return this;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetSourceTypeId(int _SourceTypeId)
        {
            m_SourceTypeId = _SourceTypeId;
            return this;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetSourceTypeUId(string _SourceTypeUId)
        {
            m_SourceTypeUId = _SourceTypeUId;
            return this;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetPersonId(int _PersonId)
        {
            m_PersonId = _PersonId;
            return this;
        }
        //--------------------------------------------------------------------------
        public MessengerSendAction SetPersonPhoneNumber(string _PersonPhoneNumber)
        {
            m_PersonPhoneNumber = _PersonPhoneNumber;
            return this;
        }
        //--------------------------------------------------------------------------
        public void SetWhatsAppAsFirstSendingVariant(Tuple<int, string> _WhatsAppData)
        {
            if (_WhatsAppData?.Item2 == null)
            {
                m_MessengerSendMethod = MessengerSendMethods.None;
                m_MessengerType = null;
                m_SourceTypeId = -1;
                m_SourceTypeUId = null;
                return;
            }

            m_MessengerSendMethod = MessengerSendMethods.FirstSendMessage;
            m_MessengerType = MessengersType.WhatsApp;
            m_SourceTypeId = _WhatsAppData.Item1;
            m_SourceTypeUId = _WhatsAppData.Item2;
        }
        //--------------------------------------------------------------------------
        public MessengerDialogCreateContent CreateMessengerDialogCreateContent()
        {
            MessengerDialogCreateContent _Result = new MessengerDialogCreateContent(DateTime.UtcNow)
                .SetSourceTypeId(m_SourceTypeId).SetPersonId(m_PersonId);

            if (m_MessengerSendMethod == MessengerSendMethods.FirstSendMessage)
            {
                _Result.SetMessengerDialogData(m_PersonPhoneNumber);
            }
            else if (m_MessengerSendMethod == MessengerSendMethods.SendMessage)
            {
                _Result.MessengerDialogId = m_MessengerDialogId;
            }

            _Result.MessageText = m_TextMessage;

            _Result.Guid = m_Guid;

            return _Result;
        }
        //--------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------
    public enum MessengerSendMethods
    {
        SendMessage,
        FirstSendMessage,
        None
    }
    //------------------------------------------------------------------------------
}
