using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class MessengerDialogFillerSiMedDB : AbstractFillerSiMedDB
    {
        private Message m_Message;
        private ChatDialog m_ChatDialog;
        private dtsChatMessenger.MESSENGER_DIALOGRow m_MessengerDialogRow;
        //-----------------------------------------------------
        public MessengerDialogFillerSiMedDB(DBLogger _DBLogger, Message _Message) : base(_DBLogger)
        {
            m_Message = _Message;
            m_ChatDialog = _Message.ChatDialog;
        }
        //-----------------------------------------------------
        protected override void SetDataTable()
        {
            m_DataTable = m_DBLogger.MESSENGER_DIALOGDataTable;
        }
        //-----------------------------------------------------
        protected override void SetIsNewRow()
        {
            List<dtsChatMessenger.MESSENGER_DIALOGRow> _MessengerDialogRows = new List<dtsChatMessenger.MESSENGER_DIALOGRow>();

            if (m_Message.MessageType == MessageType.Incoming)
            {
                foreach (dtsChatMessenger.MESSENGER_DIALOGRow row in (dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable)
                {
                    if (!row.IsNull("MES_DIAL_EXTERNAL_ID") && row.MES_DIAL_EXTERNAL_ID == m_ChatDialog.Id)
                    {
                        _MessengerDialogRows.Add(row);
                    }
                }

                int _MessengerDialogId;

                if (_MessengerDialogRows.Count > 0)
                {
                    _MessengerDialogId = _MessengerDialogRows.Max(row => row.MES_DIAL_ID);

                    m_MessengerDialogRow = _MessengerDialogRows.First(row => row.MES_DIAL_ID == _MessengerDialogId);

                    m_IsNewRow = _MessengerDialogRows.Count == 0;
                }
                else
                {
                    foreach (dtsChatMessenger.MESSENGER_DIALOGRow row in (dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable)
                    {
                        bool result = false;

                        if (!row.IsNull("MES_DIAL_CLIENT_PHONE") && m_ChatDialog.Customer.Phone != null)
                        {
                            result |= CheckEqualPhoneNumber(m_ChatDialog.Customer.Phone, row.MES_DIAL_CLIENT_PHONE);
                        }

                        if (!row.IsNull("MES_DIAL_CLIENT_PHONE"))
                        {
                            result |= CheckEqualPhoneNumber(m_ChatDialog.Customer.Login, row.MES_DIAL_CLIENT_PHONE);
                        }

                        if (m_ChatDialog.Customer.Phone != null)
                        {
                            result |= CheckEqualPhoneNumber(m_ChatDialog.Customer.Phone, row.MES_DIAL_CLIENT_LOGIN);
                        }

                        result |= CheckEqualPhoneNumber(m_ChatDialog.Customer.Login, row.MES_DIAL_CLIENT_LOGIN);

                        result &= row.SOURCE_TYPERow.SOURCE_TYPE_UID == m_ChatDialog.MessengerAccount.Id;

                        if (result)
                        {
                            m_MessengerDialogRow = ((dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable)
                                .FindByMES_DIAL_ID(row.MES_DIAL_ID);

                            m_IsNewRow = false;
                        }
                    }                   
                }
            }
            else
            {
                dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow _MessengerDialogMessageRow = null;

                foreach (dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow row in m_DBLogger.MESSENGER_DIALOG_MESSAGEDataTable.Rows)
                {
                    if (!row.IsNull("MES_DIAL_MES_GUID") && row.MES_DIAL_MES_GUID == m_Message.GUID)
                    {
                        _MessengerDialogMessageRow = row;
                    }
                }

                if (_MessengerDialogMessageRow != null)
                {
                    m_MessengerDialogRow = ((dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable)
                        .FindByMES_DIAL_ID(_MessengerDialogMessageRow.MES_DIAL_ID);
                }

                m_IsNewRow = false;
            }

            if (m_MessengerDialogRow == null)
            {
                m_IsNewRow = true;
            }
        }
        //-----------------------------------------------------
        protected override void CreateNewRow()
        {
            m_MessengerDialogRow = ((dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable).NewMESSENGER_DIALOGRow();
        }
        //-----------------------------------------------------
        protected override void FillDataInRow()
        {
            if (m_IsNewRow)
            {
                // Если диалог новый, то устанавливаем ему статус "Новый"
                m_MessengerDialogRow.MES_DIAL_STAT_TYPE_ID = 1;

                m_MessengerDialogRow.MES_DIAL_CREATE_DATE = DateTime.Now;

                TrySetPersonMessengerTypeId();
            }

            m_MessengerDialogRow.SOURCE_TYPE_ID = m_DBLogger.SOURCE_TYPEDataTable
                .Cast<dtsChatMessenger.SOURCE_TYPERow>()
                .First(r => r.SOURCE_TYPE_UID == m_ChatDialog.MessengerAccount.Id).SOURCE_TYPE_ID;

            m_MessengerDialogRow.MES_DIAL_TYPE_ID = GetMessengerDialogTypeIdFromDialogType(m_ChatDialog.DialogType);

            m_MessengerDialogRow.MES_DIAL_EXTERNAL_ID = m_ChatDialog.Id;

            m_MessengerDialogRow.MES_DIAL_MESSAGE_IS_READ = m_Message.MessageType == MessageType.Outgoing ? true : false;

            m_MessengerDialogRow.MES_DIAL_IS_BLOCKING = false;

            Customer customer = m_ChatDialog.Customer;

            bool _NeedContinue = true;

            if (customer != null)
            {
                m_MessengerDialogRow.MES_DIAL_CUSTOMER_UID = customer.Id;
                m_MessengerDialogRow.MES_DIAL_CLIENT_LOGIN = customer.Login;
                m_MessengerDialogRow.MES_DIAL_CLIENT_NAME = customer.Name;
                m_MessengerDialogRow.MES_DIAL_CLIENT_PHONE = customer.Phone;
                m_MessengerDialogRow.MES_DIAL_CLIENT_EMAIL = customer.Email;
                m_MessengerDialogRow.MES_DIAL_CLIENT_URL = customer.ProfileURL;
            }
            else if (m_IsNewRow)
            {
                _NeedContinue = false;
            }

            if (_NeedContinue)
            {
                if (m_ChatDialog.AnswerDateTime != null)
                {
                    m_MessengerDialogRow.MES_DIAL_ANSWER_DATE = new DateTime(1970, 1, 1).AddSeconds((long)m_ChatDialog.AnswerDateTime);
                }

                if (m_IsNewRow)
                {
                    ((dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable).AddMESSENGER_DIALOGRow(m_MessengerDialogRow);
                }
            }
        }
        //-----------------------------------------------------
        private void TrySetPersonMessengerTypeId()
        {
            string _LeadId = m_ChatDialog.Id.Split('\n')[0];

            dtsChatMessenger.MESSENGER_DIALOGRow _MessengerDialogRow = null;

            foreach (dtsChatMessenger.MESSENGER_DIALOGRow row in (dtsChatMessenger.MESSENGER_DIALOGDataTable)m_DataTable)
            {
                if (!row.IsNull("MES_DIAL_EXTERNAL_ID") && row.MES_DIAL_EXTERNAL_ID.Split('\n')[0] == _LeadId)
                {
                    _MessengerDialogRow = row;
                    break;
                }
            }

            if (_MessengerDialogRow != null && !_MessengerDialogRow.IsNull("PER_MES_TYPE_ID"))
            {
                m_MessengerDialogRow.PER_MES_TYPE_ID = _MessengerDialogRow.PER_MES_TYPE_ID;
            }
        }
        //-----------------------------------------------------
    }
}
