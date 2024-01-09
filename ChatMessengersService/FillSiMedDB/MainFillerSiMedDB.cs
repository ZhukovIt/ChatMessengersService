using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatMessengersService.FillSiMedDB
{
    public sealed class MainFillerSiMedDB
    {
        private DBLogger m_DBLogger;
        private Message m_Message;
        private Func<Action, string, bool> m_DoAction;
        private IEnumerable<AbstractFillerSiMedDB> m_FillersSiMedDB;
        private IMessengerCommon m_ChatMessenger;
        //-----------------------------------------------------
        public Message Message
        {
            set
            {
                m_Message = value;

                IEnumerable<SourceMessage> _SourceMessages = null;

                try
                {
                    _SourceMessages = m_ChatMessenger.GetSourceMessages();
                }
                catch (InvalidOperationException)
                {
                    m_ChatMessenger.Init(ClinicChatAggregatorApplicationType.Service);
                    _SourceMessages = m_ChatMessenger.GetSourceMessages();
                }

                var items = new List<AbstractFillerSiMedDB>
                {
                    new SourceTypeFillerSiMedDB(m_DBLogger, _SourceMessages),
                    new MessengerDialogFillerSiMedDB(m_DBLogger, value),
                    new PersonMessengerTypesFillerSiMedDB(m_DBLogger, value.ChatDialog),
                    new MessengerDialogMessageFillerSiMedDB(m_DBLogger, value)
                };

                foreach (Attachment attachment in value.Attachments)
                {
                    items.Add(new MessengerDialogMessageAttachmentFillerSiMedDB(m_DBLogger, value, attachment));
                }

                m_FillersSiMedDB = items;
            }
        }
        //-----------------------------------------------------
        public MainFillerSiMedDB(Func<Action, string, bool> _DoAction, DBLogger _DBLogger, IMessengerCommon _ChatMessenger)
        {
            m_DoAction = _DoAction;
            m_DBLogger = _DBLogger;
            m_ChatMessenger = _ChatMessenger;
        }
        //-----------------------------------------------------
        public bool FillDB()
        {
            return m_DoAction.Invoke(() =>
            {
                foreach (AbstractFillerSiMedDB fillerSiMedDB in m_FillersSiMedDB)
                {
                    fillerSiMedDB.FillComponent();
                    m_DBLogger.Save();
                }
            },
            "Не удалось корректно заполнить данные в таблицы БД");
        }
        //-----------------------------------------------------
    }
}
