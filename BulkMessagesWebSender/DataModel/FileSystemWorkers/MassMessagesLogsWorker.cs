using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BulkMessagesWebServer.DataModel
{
    public sealed class MassMessagesLogsWorker
    {
        private readonly FileSystemWorker m_FileSystemWorker;
        //--------------------------------------------------------
        public MassMessagesLogsWorker(string _StartupPath)
        {
            m_FileSystemWorker = new FileSystemWorker(_StartupPath, "MassMessagesLogs");
        }
        //--------------------------------------------------------
        public void SaveMessage(LogData _LogData)
        {
            if (_LogData.IsError)
            {
                m_FileSystemWorker.SaveExceptionToLogFile(_LogData);
            }
            else
            {
                m_FileSystemWorker.SaveRecordToLogFile(_LogData);
            }
        }
        //--------------------------------------------------------
    }
}
