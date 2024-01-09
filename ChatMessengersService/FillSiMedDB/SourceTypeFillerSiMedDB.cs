using SiMed.Clinic.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class SourceTypeFillerSiMedDB : AbstractFillerSiMedDB
    {
        private IEnumerable<SourceMessage> m_SourceMessages;
        //-----------------------------------------------------
        public SourceTypeFillerSiMedDB(DBLogger _DBLogger, IEnumerable<SourceMessage> _SourceMessages) : base(_DBLogger)
        {
            m_SourceMessages = _SourceMessages;
        }
        //-----------------------------------------------------
        protected override void SetDataTable()
        {
            m_DataTable = m_DBLogger.SOURCE_TYPEDataTable;
        }
        //-----------------------------------------------------
        protected override void SetIsNewRow()
        {
            m_IsNewRow = false;
        }
        //-----------------------------------------------------
        protected override void CreateNewRow()
        {
            // Данный метод никогда не будет вызван (см. метод SetIsNewRow)
        }
        //-----------------------------------------------------
        protected override void FillDataInRow()
        {
            List<SourceMessage> _NewSourceMessages = new List<SourceMessage>();
            dtsChatMessenger.SOURCE_TYPEDataTable _SourceTypeDataTable = (dtsChatMessenger.SOURCE_TYPEDataTable)m_DataTable;

            foreach (SourceMessage _SourceMessage in m_SourceMessages)
            {
                if (_SourceTypeDataTable.FirstOrDefault(row => row.SOURCE_TYPE_UID == _SourceMessage.Id) == null)
                {
                    _NewSourceMessages.Add(_SourceMessage);
                }
            }

            foreach (SourceMessage _SourceMessage in _NewSourceMessages)
            {
                dtsChatMessenger.SOURCE_TYPERow _NewSourceTypeRow = _SourceTypeDataTable.NewSOURCE_TYPERow();

                _NewSourceTypeRow.MESSENGER_TYPE_ID = GetMessengerTypeId(_SourceMessage.Type);
                _NewSourceTypeRow.SOURCE_TYPE_UID = _SourceMessage.Id;
                _NewSourceTypeRow.SOURCE_TYPE_ACC_NAME = _SourceMessage.Login;

                _SourceTypeDataTable.AddSOURCE_TYPERow(_NewSourceTypeRow);
            }
        }
        //-----------------------------------------------------
    }
}
