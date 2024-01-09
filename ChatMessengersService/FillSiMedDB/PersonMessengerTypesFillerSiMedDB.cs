using SiMed.Clinic.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace ChatMessengersService.FillSiMedDB
{
    public class PersonMessengerTypesFillerSiMedDB : AbstractFillerSiMedDB
    {
        private ChatDialog m_ChatDialog;
        private dtsChatMessenger.PERSON_MESSENGER_TYPESRow m_PersonMessengerTypesRow;
        //-----------------------------------------------------
        public PersonMessengerTypesFillerSiMedDB(DBLogger _DBLogger, ChatDialog _ChatDialog) : base(_DBLogger)
        {
            m_ChatDialog = _ChatDialog;
        }
        //-----------------------------------------------------
        protected override bool CheckCorrectHandleRequest()
        {
            return m_ChatDialog.Customer != null;
        }
        //-----------------------------------------------------
        protected override void SetDataTable()
        {
            m_DataTable = m_DBLogger.PERSON_MESSENGER_TYPESDataTable;
        }
        //-----------------------------------------------------
        protected override void SetIsNewRow()
        {
            List<dtsChatMessenger.MESSENGER_DIALOGRow> _MessengerDialogRows = new List<dtsChatMessenger.MESSENGER_DIALOGRow>();

            foreach (dtsChatMessenger.MESSENGER_DIALOGRow row in m_DBLogger.MESSENGER_DIALOGDataTable)
            {
                if (!row.IsNull("MES_DIAL_EXTERNAL_ID") && row.MES_DIAL_EXTERNAL_ID == m_ChatDialog.Id)
                {
                    _MessengerDialogRows.Add(row);
                }
            }

            if (_MessengerDialogRows.Count > 0)
            {
                int _MessengerDialogMaxId = _MessengerDialogRows.Max(row => row.MES_DIAL_ID);

                dtsChatMessenger.MESSENGER_DIALOGRow dialogRow = _MessengerDialogRows.First(row => row.MES_DIAL_ID == _MessengerDialogMaxId);

                if (!dialogRow.IsNull("PER_MES_TYPE_ID"))
                {
                    m_PersonMessengerTypesRow = m_DBLogger.PERSON_MESSENGER_TYPESDataTable.FindByPER_MES_TYPE_ID(dialogRow.PER_MES_TYPE_ID);
                }
            }

            m_IsNewRow = false;
        }
        //-----------------------------------------------------
        protected override void CreateNewRow()
        {
            // Данный метод никогда не будет вызван! (см. метод SetIsNewRow)
        }
        //-----------------------------------------------------
        protected override void FillDataInRow()
        {
            if (m_PersonMessengerTypesRow != null)
            {
                m_PersonMessengerTypesRow.PER_MES_TYPE_UID = m_ChatDialog.Customer.Id;

                if (!string.IsNullOrEmpty(m_ChatDialog.Customer.Avatar?.Trim()))
                {
                    m_PersonMessengerTypesRow.PER_MES_TYPE_CLIENT_PHOTO = m_ChatDialog.Customer.Avatar;
                }
            }
        }
        //-----------------------------------------------------
    }
}
