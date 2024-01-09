using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class DBLogger
    {
        private dtsChatMessenger m_dtsChatMessenger;
        private SqlConnection MainConnection;
        private dtsChatMessengerTableAdapters.TableAdapterManager m_taManager;
        private Func<Action, string, bool> m_DoAction;
        //-----------------------------------------------------
        private dtsChatMessengerTableAdapters.MESSENGER_DIALOG_MESSAGETableAdapter m_taMessengerDialogMessage;
        private dtsChatMessengerTableAdapters.MESSENGER_DIALOGTableAdapter m_taMessengerDialog;
        private dtsChatMessengerTableAdapters.MESSENGER_DIALOG_MESSAGE_ATTACHMENTTableAdapter m_taMessengerDialogMessageAttachment;
        private dtsChatMessengerTableAdapters.SOURCE_TYPETableAdapter m_taSourceType;
        private dtsChatMessengerTableAdapters.PERSON_MESSENGER_TYPESTableAdapter m_taPersonMessengerTypes;
        private dtsChatMessengerTableAdapters.MESSENGER_TYPETableAdapter m_taMessengerType;
        //-----------------------------------------------------
        public DBLogger(Func<Action, string, bool> _DoAction, bool _ClearBeforeFill = true)
        {
            m_DoAction = _DoAction;
            m_dtsChatMessenger = new dtsChatMessenger();
            MainConnection = new SqlConnection(GetConnectionString());

            m_taMessengerDialogMessage = new dtsChatMessengerTableAdapters.MESSENGER_DIALOG_MESSAGETableAdapter();
            m_taMessengerDialogMessage.Connection = MainConnection;
            m_taMessengerDialogMessage.ClearBeforeFill = _ClearBeforeFill;

            m_taMessengerDialog = new dtsChatMessengerTableAdapters.MESSENGER_DIALOGTableAdapter();
            m_taMessengerDialog.Connection = MainConnection;
            m_taMessengerDialog.ClearBeforeFill = _ClearBeforeFill;

            m_taMessengerDialogMessageAttachment = new dtsChatMessengerTableAdapters.MESSENGER_DIALOG_MESSAGE_ATTACHMENTTableAdapter();
            m_taMessengerDialogMessageAttachment.Connection = MainConnection;
            m_taMessengerDialogMessageAttachment.ClearBeforeFill = _ClearBeforeFill;

            m_taSourceType = new dtsChatMessengerTableAdapters.SOURCE_TYPETableAdapter();
            m_taSourceType.Connection = MainConnection;
            m_taSourceType.ClearBeforeFill = _ClearBeforeFill;

            m_taPersonMessengerTypes = new dtsChatMessengerTableAdapters.PERSON_MESSENGER_TYPESTableAdapter();
            m_taPersonMessengerTypes.Connection = MainConnection;
            m_taPersonMessengerTypes.ClearBeforeFill = _ClearBeforeFill;

            m_taMessengerType = new dtsChatMessengerTableAdapters.MESSENGER_TYPETableAdapter();
            m_taMessengerType.Connection = MainConnection;
            m_taMessengerType.ClearBeforeFill = _ClearBeforeFill;

            m_taManager = new dtsChatMessengerTableAdapters.TableAdapterManager();
            m_taManager.Connection = MainConnection;
            m_taManager.MESSENGER_DIALOG_MESSAGETableAdapter = m_taMessengerDialogMessage;
            m_taManager.MESSENGER_DIALOGTableAdapter = m_taMessengerDialog;
            m_taManager.MESSENGER_DIALOG_MESSAGE_ATTACHMENTTableAdapter = m_taMessengerDialogMessageAttachment;
            m_taManager.SOURCE_TYPETableAdapter = m_taSourceType;
            m_taManager.PERSON_MESSENGER_TYPESTableAdapter = m_taPersonMessengerTypes;
        }
        //-----------------------------------------------------
        public bool Load()
        {
            return m_DoAction.Invoke(() =>
            {
                m_taMessengerDialogMessage.Fill(m_dtsChatMessenger.MESSENGER_DIALOG_MESSAGE);
                m_taMessengerDialog.Fill(m_dtsChatMessenger.MESSENGER_DIALOG);
                m_taMessengerDialogMessageAttachment.Fill(m_dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENT);
                m_taSourceType.Fill(m_dtsChatMessenger.SOURCE_TYPE);
                m_taPersonMessengerTypes.Fill(m_dtsChatMessenger.PERSON_MESSENGER_TYPES);
                m_taMessengerType.Fill(m_dtsChatMessenger.MESSENGER_TYPE);
            },
            "Данные из БД не загружены или загружены не корректно");
        }
        //-----------------------------------------------------
        public bool Save()
        {
            return m_DoAction.Invoke(() =>
            {
                m_taManager.UpdateAll(m_dtsChatMessenger);
            },
            "Данные в БД не сохранились");
        }
        //-----------------------------------------------------
        public dtsChatMessenger.SOURCE_TYPEDataTable SOURCE_TYPEDataTable =>
            m_dtsChatMessenger.SOURCE_TYPE;
        //-----------------------------------------------------
        public dtsChatMessenger.PERSON_MESSENGER_TYPESDataTable PERSON_MESSENGER_TYPESDataTable =>
            m_dtsChatMessenger.PERSON_MESSENGER_TYPES;
        //-----------------------------------------------------
        public dtsChatMessenger.MESSENGER_DIALOGDataTable MESSENGER_DIALOGDataTable =>
            m_dtsChatMessenger.MESSENGER_DIALOG;
        //-----------------------------------------------------
        public IEnumerable<string> GetMessengerDialogUIdsForBlockingMessengerDialogs()
        {
            List<string> _Result = new List<string>();

            foreach (var row in m_dtsChatMessenger.MESSENGER_DIALOG)
            {
                if (row.MES_DIAL_IS_BLOCKING && !row.IsMES_DIAL_EXTERNAL_IDNull())
                {
                    _Result.Add(row.MES_DIAL_EXTERNAL_ID);
                }
            }

            return _Result;
        }
        //-----------------------------------------------------
        public dtsChatMessenger.MESSENGER_DIALOG_MESSAGEDataTable MESSENGER_DIALOG_MESSAGEDataTable =>
            m_dtsChatMessenger.MESSENGER_DIALOG_MESSAGE;
        //-----------------------------------------------------
        public dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable MESSENGER_DIALOG_MESSAGE_ATTACHMENTDataTable =>
            m_dtsChatMessenger.MESSENGER_DIALOG_MESSAGE_ATTACHMENT;
        //-----------------------------------------------------
        public dtsChatMessenger.MESSENGER_TYPEDataTable MESSENGER_TYPEDataTable =>
            m_dtsChatMessenger.MESSENGER_TYPE;
        //-----------------------------------------------------
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ChatMessengersService.Properties.Settings.dtsChatMessenger"].ConnectionString;
        }
        //-----------------------------------------------------
    }
}
