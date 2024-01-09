using System.IO;
using System.Linq;

namespace BulkMessagesWebServer.DataModel.FileSystem.Images
{
    public sealed class ImagesPersister
    {
        public FileImageContent[] ReadDirectory(string _DirectoryName)
        {
            return Directory
                .GetFiles(_DirectoryName)
                .Select(x => new FileImageContent(
                    Path.GetFileName(x),
                    new FileInfo(x).CreationTime))
                .ToArray();
        }
        //---------------------------------------------------------------
        public void ApplyAction(string _DirectoryName, FileImageAction _FileAction)
        {
            string _FilePath = Path.Combine(_DirectoryName, _FileAction.FileName);
            FileActionTypes _FileActionType = _FileAction.FileActionType;

            if (_FileActionType == FileActionTypes.Delete)
            {
                File.Delete(_FilePath);
            }
            else
            {
                File.WriteAllBytes(_FilePath, _FileAction.FileData);
            }
        }
        //---------------------------------------------------------------
        public bool DirectoryIsExists(string _DirectoryName)
        {
            return Directory.Exists(_DirectoryName);
        }
        //---------------------------------------------------------------
        public void CreateDirectory(string _DirectoryName)
        {
            Directory.CreateDirectory(_DirectoryName);
        }
        //---------------------------------------------------------------
    }
}
