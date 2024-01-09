using System;

namespace BulkMessagesWebServer.DataModel.FileSystem.Images
{
    public sealed class FileImageContent
    {
        private readonly string m_FileName;
        private readonly DateTime m_CreationDateTime;
        //----------------------------------------------------
        public string FileName
        {
            get
            {
                return m_FileName;
            }
        }
        //----------------------------------------------------
        public DateTime CreationDateTime
        {
            get
            {
                return m_CreationDateTime;
            }
        }
        //----------------------------------------------------
        public FileImageContent(string _FileName, DateTime _CreationDateTime)
        {
            m_FileName = _FileName;
            m_CreationDateTime = _CreationDateTime;
        }
        //----------------------------------------------------
    }
}
