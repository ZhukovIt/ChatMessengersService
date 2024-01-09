using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DataModel
{
    public sealed class FileSystemWorker
    {
        private readonly string m_StartupPath;
        private readonly string m_DirectoryName;
        private readonly string m_DirectoryPath;
        //-----------------------------------------------------------
        public FileSystemWorker(string _StartupPath, string _DirectoryName)
        {
            m_StartupPath = _StartupPath;
            m_DirectoryName = _DirectoryName;
            m_DirectoryPath = $"{_StartupPath}\\{_DirectoryName}";
        }
        //-----------------------------------------------------------
        public void CheckExistsDirectory()
        {
            if (!Directory.Exists(m_DirectoryPath))
            {
                Directory.CreateDirectory(m_DirectoryPath);
            }
        }
        //-----------------------------------------------------------
        public void CheckExistsSubDirectory(string _SubDirectoryName)
        {
            string _SubDirectoryPath = $"{m_DirectoryPath}\\{_SubDirectoryName}";

            if (!Directory.Exists(_SubDirectoryPath))
            {
                Directory.CreateDirectory(_SubDirectoryPath);
            }
        }
        //-----------------------------------------------------------
        public void SaveExceptionToLogFile(LogData _LogData)
        {
            string _SubDirectoryName = "ExceptionLogs";

            string _FullFilePath = $"{m_DirectoryPath}\\{_SubDirectoryName}" + 
                $"\\Ошибки на сервере за {_LogData.FormattedLogDateWithDashSeparator}.log";

            CheckExistsDirectory();

            CheckExistsSubDirectory(_SubDirectoryName);

            WriteExceptionLogDataFromPath(_FullFilePath, _LogData);
        }
        //-----------------------------------------------------------
        public void SaveRecordToLogFile(LogData _LogData)
        {
            string _SubDirectoryName = "ServerInfoLogs";

            string _FullFilePath = $"{m_DirectoryPath}\\{_SubDirectoryName}" +
                $"\\Данные от сервера за {_LogData.FormattedLogDateWithDashSeparator}.log";

            CheckExistsDirectory();

            CheckExistsSubDirectory(_SubDirectoryName);

            WriteInfoLogDataFromPath(_FullFilePath, _LogData);
        }
        //-----------------------------------------------------------
        private string CreateMinusSeparatorString(int _CountMinus)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < _CountMinus; i++)
            {
                builder.Append('-');
            }

            return builder.ToString();
        }
        //-----------------------------------------------------------
        private string CreateEqualSeparatorString(int _CountMinus)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < _CountMinus; i++)
            {
                builder.Append('=');
            }

            return builder.ToString();
        }
        //-----------------------------------------------------------
        private void WriteExceptionLogDataFromPath(string _Path, LogData _LogData)
        {
            using (StreamWriter _Writer = new StreamWriter(_Path, true, Encoding.UTF8))
            {
                _Writer.WriteLine(CreateEqualSeparatorString(120));

                StringBuilder builder = new StringBuilder();
                builder.Append($"| Дата: {_LogData.FormattedLogDateWithDotSeparator} г. ");
                builder.Append($"| Время: {_LogData.FormattedLogTimeWithColonSeparator} ");
                builder.Append($"| Тип ошибки: {_LogData.ExceptionType.FullName} |");
                _Writer.WriteLine(builder.ToString());

                _Writer.WriteLine(CreateMinusSeparatorString(120));

                _Writer.WriteLine($"Источник ошибки: {_LogData.Source}");

                _Writer.WriteLine(CreateMinusSeparatorString(120));

                _Writer.WriteLine($"Сообщение ошибки: {_LogData.Message}");

                _Writer.WriteLine(CreateMinusSeparatorString(120));

                _Writer.WriteLine($"Трассировка стека: {_LogData.StackTrace}");

                if (_LogData.DeveloperComment != null)
                {
                    _Writer.WriteLine(CreateMinusSeparatorString(120));

                    _Writer.WriteLine($"Комментарий разработчика: {_LogData.DeveloperComment}");
                }

                _Writer.WriteLine(CreateEqualSeparatorString(120));

                _Writer.WriteLine();

                _Writer.WriteLine();

                _Writer.WriteLine();
            }
        }
        //-----------------------------------------------------------
        private void WriteInfoLogDataFromPath(string _Path, LogData _LogData)
        {
            using (StreamWriter _Writer = new StreamWriter(_Path, true, Encoding.UTF8))
            {
                _Writer.WriteLine(CreateEqualSeparatorString(120));

                StringBuilder builder = new StringBuilder();
                builder.Append($"| Дата: {_LogData.FormattedLogDateWithDotSeparator} г. ");
                builder.Append($"| Время: {_LogData.FormattedLogTimeWithColonSeparator} |");
                _Writer.WriteLine(builder.ToString());

                _Writer.WriteLine(CreateMinusSeparatorString(120));

                _Writer.WriteLine($"Источник: {_LogData.Source}");

                _Writer.WriteLine(CreateMinusSeparatorString(120));

                _Writer.WriteLine($"Текст сообщения: {_LogData.Message}");

                if (_LogData.DeveloperComment != null)
                {
                    _Writer.WriteLine(CreateMinusSeparatorString(120));

                    _Writer.WriteLine($"Комментарий разработчика: {_LogData.DeveloperComment}");
                }

                _Writer.WriteLine(CreateEqualSeparatorString(120));

                _Writer.WriteLine();

                _Writer.WriteLine();

                _Writer.WriteLine();
            }
        }
        //-----------------------------------------------------------
    }
}
