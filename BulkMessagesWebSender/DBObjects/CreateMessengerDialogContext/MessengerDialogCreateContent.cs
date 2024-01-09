using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.CreateMessengerDialog
{
    public sealed class MessengerDialogCreateContent
    {
        private int m_SourceTypeId;
        private int m_PersonId;
        private string m_PhoneNumber;
        private readonly DateTime m_CreationDateTime;
        private bool m_NeedCreateMessengerDialog;
        private int m_MessengerDialogId;
        private string m_MessageText;
        private string m_Guid;
        private string m_FileName;
        private string m_FileData;
        private bool m_NeedCreateMessengerDialogMessageAttachment;
        private int m_MessengerTypeId;
        private int m_PersonMessengerTypeId;
        private bool m_MessengerDialogHasNotPersonMessengerTypeId;
        //------------------------------------------------------------------
        public bool MessengerDialogHasNotPersonMessengerTypeId
        {
            get => m_MessengerDialogHasNotPersonMessengerTypeId;
            set => m_MessengerDialogHasNotPersonMessengerTypeId = value;
        }
        //------------------------------------------------------------------
        public int PersonMessengerTypeId
        {
            get => m_PersonMessengerTypeId;
            set => m_PersonMessengerTypeId = value;
        }
        //------------------------------------------------------------------
        public int MessengerTypeId
        {
            get => m_MessengerTypeId;
            set => m_MessengerTypeId = value;
        }
        //------------------------------------------------------------------
        public bool NeedCreateMessengerDialog
        {
            get => m_NeedCreateMessengerDialog;
        }
        //------------------------------------------------------------------
        public bool NeedCreateMessengerDialogMessageAttachment
        {
            get => m_NeedCreateMessengerDialogMessageAttachment;
        }
        //------------------------------------------------------------------
        public int SourceTypeId
        {
            get => m_SourceTypeId;
        }
        //------------------------------------------------------------------
        public int PersonId
        {
            get => m_PersonId;
        }
        //------------------------------------------------------------------
        public string PhoneNumber
        {
            get => m_PhoneNumber;
        }
        //------------------------------------------------------------------
        public DateTime CreationDateTime
        {
            get => m_CreationDateTime;
        }
        //------------------------------------------------------------------
        public int MessengerDialogId
        {
            get => m_MessengerDialogId;
            set => m_MessengerDialogId = value;
        }
        //------------------------------------------------------------------
        public string MessageText
        {
            get => m_MessageText;
            set => m_MessageText = value;
        }
        //------------------------------------------------------------------
        public string Guid
        {
            get => m_Guid;
            set => m_Guid = value;
        }
        //------------------------------------------------------------------
        public string FileName
        {
            get => m_FileName;
        }
        //------------------------------------------------------------------
        public string FileData
        {
            get => m_FileData;
        }
        //------------------------------------------------------------------
        public MessengerDialogCreateContent(DateTime _CreationDateTime)
        {
            m_CreationDateTime = _CreationDateTime;
            m_NeedCreateMessengerDialog = false;
            m_NeedCreateMessengerDialogMessageAttachment = false;
        }
        //------------------------------------------------------------------
        public MessengerDialogCreateContent SetSourceTypeId(int _SourceTypeId)
        {
            m_SourceTypeId = _SourceTypeId;
            return this;
        }
        //------------------------------------------------------------------
        public MessengerDialogCreateContent SetPersonId(int _PersonId)
        {
            m_PersonId = _PersonId;
            return this;
        }
        //------------------------------------------------------------------
        public void SetMessengerDialogData(string _PhoneNumber)
        {
            m_NeedCreateMessengerDialog = true;
            m_PhoneNumber = _PhoneNumber;
        }
        //------------------------------------------------------------------
        public void SetImageAttachmentData(string _FileName, byte[] _FileData)
        {
            m_NeedCreateMessengerDialogMessageAttachment = true;
            m_FileName = _FileName;
            m_FileData = Convert.ToBase64String(_FileData);
        }
        //------------------------------------------------------------------
    }
}
