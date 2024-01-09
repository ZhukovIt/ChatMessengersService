using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class MessengerDialogMessageAttachmentFillerSiMedDB : AbstractFillerSiMedDB
    {
        private Message m_Message;
        private Attachment m_Attachment;
        private dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTRow m_MessengerDialogMessageAttachmentRow;
        //-----------------------------------------------------
        public MessengerDialogMessageAttachmentFillerSiMedDB(DBLogger _DBLogger, Message _Message, Attachment _Attachment) : base(_DBLogger)
        {
            m_Message = _Message;
            m_Attachment = _Attachment;
        }
        //-----------------------------------------------------
        protected override void SetDataTable()
        {
            m_DataTable = m_DBLogger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable;
        }
        //-----------------------------------------------------
        protected override void SetIsNewRow()
        {
            int tempMessengerDialogMessageId = -1;

            foreach (dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow row in m_DBLogger.MESSENGER_DIALOG_MESSAGEDataTable)
            {
                if (!row.IsNull("MES_DIAL_MES_GUID") && row.MES_DIAL_MES_GUID == m_Message.GUID)
                {
                    tempMessengerDialogMessageId = row.MES_DIAL_MES_ID;
                    break;
                }
            }

            if (tempMessengerDialogMessageId != -1)
            {
                m_MessengerDialogMessageAttachmentRow = ((dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable)m_DataTable)
                    .First(r => r.MES_DIAL_MES_ID == tempMessengerDialogMessageId);
            }

            m_IsNewRow = tempMessengerDialogMessageId == -1;
        }
        //-----------------------------------------------------
        protected override void CreateNewRow()
        {
            m_MessengerDialogMessageAttachmentRow = ((dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable)m_DataTable)
                .NewMESSENGER_DIALOG_MESSAGE_ATTACHMENTRow();
        }
        //-----------------------------------------------------
        protected override void FillDataInRow()
        {
            if (m_IsNewRow)
            {
                List<dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow> _MessengerDialogMessageRows = new List<dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow>();

                foreach (dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow row in m_DBLogger.MESSENGER_DIALOG_MESSAGEDataTable)
                {
                    if (!row.IsNull("MES_DIAL_MES_EXTERNAL_ID") && row.MES_DIAL_MES_EXTERNAL_ID == m_Message.Id)
                    {
                        _MessengerDialogMessageRows.Add(row);
                    }
                }

                m_MessengerDialogMessageAttachmentRow.MES_DIAL_MES_ID = _MessengerDialogMessageRows.Max(row => row.MES_DIAL_MES_ID);

                m_MessengerDialogMessageAttachmentRow.MES_DIAL_MES_ATT_NAME = m_Attachment?.Name ?? "Неизвестное" + "." + m_Attachment.Type;

                m_MessengerDialogMessageAttachmentRow.MES_DIAL_MES_ATT_DATA = m_Attachment.Data;
            }

            m_MessengerDialogMessageAttachmentRow.MES_DIAL_MES_ATT_UID = m_Attachment.Id;

            if (m_IsNewRow)
            {
                ((dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable)m_DataTable)
                    .AddMESSENGER_DIALOG_MESSAGE_ATTACHMENTRow(m_MessengerDialogMessageAttachmentRow);
            }
        }
        //-----------------------------------------------------
    }
}
