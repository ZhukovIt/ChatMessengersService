using System;

namespace BulkMessagesWebServer.DataModel.FileSystem.Images
{
    public sealed class ImagesService
    {
        private readonly string m_DirectoryName;
        private readonly ImagesManager m_ImagesManager;
        private readonly ImagesPersister m_Persister;
        //----------------------------------------------------------
        public ImagesService(string _DirectoryName)
        {
            m_DirectoryName = _DirectoryName;
            m_ImagesManager = new ImagesManager(7);
            m_Persister = new ImagesPersister();
        }
        //----------------------------------------------------------
        public string AddImage(string _ImageStringData)
        {
            if (!m_Persister.DirectoryIsExists(m_DirectoryName))
            {
                m_Persister.CreateDirectory(m_DirectoryName);
            }

            DeleteOldImages();

            if (string.IsNullOrWhiteSpace(_ImageStringData))
            {
                return null;
            }

            FileImageContent[] _Files = m_Persister.ReadDirectory(m_DirectoryName);
            
            FileImageAction _Add = m_ImagesManager.AddImage(_Files, _ImageStringData);
            
            m_Persister.ApplyAction(m_DirectoryName, _Add);
            
            return _Add.FileName;
        }
        //----------------------------------------------------------
        private void DeleteOldImages()
        {
            FileImageContent[] _Files = m_Persister.ReadDirectory(m_DirectoryName);

            DateTime _RelationDateTimeToDeleteOldFiles = DateTime.Now;

            FileImageAction[] _DeleteFiles = m_ImagesManager.DeleteOldFiles(_Files, _RelationDateTimeToDeleteOldFiles);

            foreach (FileImageAction _DeleteFile in _DeleteFiles)
            {
                m_Persister.ApplyAction(m_DirectoryName, _DeleteFile);
            }
        }
        //----------------------------------------------------------
    }
}
