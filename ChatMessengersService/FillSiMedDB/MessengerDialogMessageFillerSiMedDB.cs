using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class MessengerDialogMessageFillerSiMedDB : AbstractFillerSiMedDB
    {
        private Message m_Message;
        private dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow m_MessengerDialogMessageRow;
        private int m_MaxLengthForMessage;
        //-----------------------------------------------------
        public MessengerDialogMessageFillerSiMedDB(DBLogger _DBLogger, Message _Message) : base(_DBLogger)
        {
            m_Message = _Message;
            m_MaxLengthForMessage = 2000;
        }
        //-----------------------------------------------------
        protected override void SetDataTable()
        {
            m_DataTable = m_DBLogger.MESSENGER_DIALOG_MESSAGEDataTable;
        }
        //-----------------------------------------------------
        protected override void SetIsNewRow()
        {
            foreach (dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow row in (dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable)m_DataTable)
            {
                if (!row.IsNull("MES_DIAL_MES_GUID") && row.MES_DIAL_MES_GUID == m_Message.GUID)
                {
                    m_MessengerDialogMessageRow = row;
                }
            }

            m_IsNewRow = m_MessengerDialogMessageRow == null;
        }
        //-----------------------------------------------------
        protected override void CreateNewRow()
        {
            m_MessengerDialogMessageRow = ((dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable)m_DataTable).NewMESSENGER_DIALOG_MESSAGERow();
        }
        //-----------------------------------------------------
        protected override void FillDataInRow()
        {
            if (!m_IsNewRow)
            {
                return;
            }

            if (m_MessengerDialogMessageRow.IsNull("MES_DIAL_MES_DEPARTURE_DATE"))
            {
                m_MessengerDialogMessageRow.MES_DIAL_MES_DEPARTURE_DATE = new DateTime(1970, 1, 1).AddSeconds(m_Message.SendDateTime);
            }

            m_MessengerDialogMessageRow.MES_TYPE_ID = m_Message.MessageType == MessageType.Incoming ? 1 : 2;

            List<dtsChatMessenger.MESSENGER_DIALOGRow> _MessengerDialogRows = new List<dtsChatMessenger.MESSENGER_DIALOGRow>();

            foreach (dtsChatMessenger.MESSENGER_DIALOGRow row in m_DBLogger.MESSENGER_DIALOGDataTable)
            {
                if (!row.IsNull("MES_DIAL_EXTERNAL_ID") && row.MES_DIAL_EXTERNAL_ID == m_Message.ChatDialog.Id)
                {
                    _MessengerDialogRows.Add(row);
                }
            }

            if (_MessengerDialogRows.Count > 0)
            {
                m_MessengerDialogMessageRow.MES_DIAL_ID = _MessengerDialogRows.Max(row => row.MES_DIAL_ID);

                m_MessengerDialogMessageRow.MES_STAT_TYPE_ID = m_Message.MessageType == MessageType.Incoming ? 2 : 6;

                m_MessengerDialogMessageRow.MES_DIAL_MES_EXTERNAL_ID = m_Message.Id;

                m_MessengerDialogMessageRow.MES_DIAL_MES_URL = m_Message.URL;

                SaveWithCorrectTextForMessengerDialogMessage(m_MessengerDialogMessageRow, m_Message.Text);
            }
        }
        //-----------------------------------------------------
        #region Вспомогательные закрытые методы
        //-----------------------------------------------------
        private void SaveWithCorrectTextForMessengerDialogMessage(dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow _Row, string _Text)
        {
            var _DataTable = (dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable)m_DataTable;
            int _MessageLength = _Text.Length;

            if (_MessageLength <= m_MaxLengthForMessage)
            {
                _Row.MES_DIAL_MES_TEXT = _Text;

                _DataTable.AddMESSENGER_DIALOG_MESSAGERow(_Row);

                return;
            }

            if (m_MaxLengthForMessage <= 3)
            {
                throw new DivideByZeroException("Поле m_MaxLengthForMessage (MessengerDialogMessageFillerSiMedDB) должно быть больше, чем 3!");
            }

            int _CountMessages = (int)((double)_MessageLength / (m_MaxLengthForMessage - 3)) + 1;

            for (int i = 1; i <= _CountMessages; i++)
            {
                int _DopCoef = i - 1 == 0 ? 0 : 1;

                string tempText;

                if (i == _CountMessages)
                {
                    tempText = _Text.Substring((i - 1) * (m_MaxLengthForMessage - 3) + _DopCoef);

                    _Row.MES_DIAL_MES_TEXT = tempText;

                    _DataTable.AddMESSENGER_DIALOG_MESSAGERow(_Row);
                }
                else
                {
                    tempText = _Text.Substring((i - 1) * (m_MaxLengthForMessage - 3) + _DopCoef, m_MaxLengthForMessage - 3);

                    dtsChatMessenger.MESSENGER_DIALOG_MESSAGERow tempRow = _Row.Clone((dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable)m_DataTable);

                    tempRow.MES_DIAL_MES_TEXT = tempText + "...";

                    _DataTable.AddMESSENGER_DIALOG_MESSAGERow(tempRow);
                }
            }
        }
        //-----------------------------------------------------
        #endregion
        //-----------------------------------------------------
    }
}
