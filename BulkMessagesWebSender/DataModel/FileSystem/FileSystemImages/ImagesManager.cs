using System;
using System.Linq;

namespace BulkMessagesWebServer.DataModel.FileSystem.Images
{
    public sealed class ImagesManager
    {
        private readonly int m_BackTimestampDaysOfOldFiles;
        //---------------------------------------------------------------------------------------
        public ImagesManager(int _BackTimestampDaysOfOldFiles)
        {
            m_BackTimestampDaysOfOldFiles = _BackTimestampDaysOfOldFiles;
        }
        //---------------------------------------------------------------------------------------
        public FileImageAction AddImage(FileImageContent[] _Files, string _StringFileData)
        {
            byte[] _FileData = Convert.FromBase64String(_StringFileData);

            if (_Files.Length == 0)
            {
                return new FileImageAction("Image1.jpg", _FileData, FileActionTypes.Add);
            }

            FileImageContent[] _SortedFiles = _Files.OrderBy(x => x.CreationDateTime).ToArray();

            FileImageContent _LastFileContent = _SortedFiles.Last();

            int _IndexLastFileContent = int.Parse(
                new string(
                    _LastFileContent.FileName.Where(c => char.IsDigit(c)).ToArray()
                )
            );

            string _FileName = $"Image{_IndexLastFileContent + 1}.jpg";

            return new FileImageAction(_FileName, _FileData, FileActionTypes.Add);
        }
        //---------------------------------------------------------------------------------------
        public FileImageAction[] DeleteOldFiles(FileImageContent[] _Files, DateTime _RelationDateTimeToDeleteOldFiles)
        {
            DateTime _DateTimeToDeleteOldFiles = _RelationDateTimeToDeleteOldFiles.AddDays(-1 * m_BackTimestampDaysOfOldFiles);

            return _Files
                .Where(x => _DateTimeToDeleteOldFiles >= x.CreationDateTime)
                .Select(x => new FileImageAction(x.FileName, FileActionTypes.Delete))
                .ToArray();
        }
        //---------------------------------------------------------------------------------------
    }
}
