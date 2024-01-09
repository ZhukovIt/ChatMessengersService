using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class ImagesPersister
    {
        private string m_DirectoryPath;
        //------------------------------------------------------------------------------------------------
        public ImagesPersister(string _DirectoryPath)
        {
            m_DirectoryPath = _DirectoryPath;
        }
        //------------------------------------------------------------------------------------------------
        public ImagesPersister(string _StartupPath, string _DirectoryName)
        {
            m_DirectoryPath = Path.Combine(_StartupPath, _DirectoryName);
        }
        //------------------------------------------------------------------------------------------------
        public string GetFullFilePathByFileName(string _FileName)
        {
            return Path.Combine(m_DirectoryPath, _FileName);
        }
        //------------------------------------------------------------------------------------------------
        public byte[] GetImageDataByFileName(string _FullFilePath)
        {
            return File.ReadAllBytes(_FullFilePath);
        }
        //------------------------------------------------------------------------------------------------
    }
}
