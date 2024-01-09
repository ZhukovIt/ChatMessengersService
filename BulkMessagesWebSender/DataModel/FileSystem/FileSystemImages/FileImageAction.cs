using System;

namespace BulkMessagesWebServer.DataModel.FileSystem.Images
{
    public sealed class FileImageAction
    {
        private readonly string m_FileName;
        private readonly byte[] m_FileData;
        private readonly FileActionTypes m_FileActionType;
        //-----------------------------------------------
        public string FileName
        {
            get
            {
                return m_FileName;
            }
        }
        //-----------------------------------------------
        public byte[] FileData
        {
            get
            {
                return m_FileData;
            }
        }
        //-----------------------------------------------
        public FileActionTypes FileActionType
        {
            get
            {
                return m_FileActionType;
            }
        }
        //-----------------------------------------------
        public FileImageAction(string _FileName, FileActionTypes _FileActionType)
        {
            m_FileName = _FileName;
            m_FileActionType = _FileActionType;
        }
        //-----------------------------------------------
        public FileImageAction(string _FileName, byte[] _FileData, FileActionTypes _FileActionType)
        {
            m_FileName = _FileName;
            m_FileData = _FileData;
            m_FileActionType = _FileActionType;
        }
        //-----------------------------------------------
    }
}
