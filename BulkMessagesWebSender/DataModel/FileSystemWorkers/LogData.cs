using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DataModel
{
    public class LogData
    {
        private bool m_IsError;
        private Type m_ExceptionType;
        private string m_Source;
        private string m_Message;
        private string m_StackTrace;
        private string m_FilePath;
        private readonly DateTime m_LogDateTime;
        private string m_FormattedLogDateWithDotSeparator;
        private string m_FormattedLogDateWithDashSeparator;
        private string m_FormattedLogTimeWithColonSeparator;
        private string m_DeveloperComment;
        //--------------------------------------------------
        public LogData()
        {
            m_IsError = false;
            m_ExceptionType = null;
            m_Source = null;
            m_StackTrace = null;
            m_LogDateTime = DateTime.Now;
            FormatDate(m_LogDateTime);
            FormatTime(m_LogDateTime);
        }
        //--------------------------------------------------
        public LogData(string _FilePath) : this()
        {
            m_FilePath = _FilePath;
        }
        //--------------------------------------------------
        public string FilePath
        {
            get
            {
                return m_FilePath;
            }

            set
            {
                m_FilePath = value;
            }
        }
        //--------------------------------------------------
        public bool IsError
        {
            get
            {
                return m_IsError;
            }
        }
        //--------------------------------------------------
        public Type ExceptionType
        {
            get
            {
                return m_ExceptionType;
            }
        }
        //--------------------------------------------------
        public string Source
        {
            get
            {
                return m_Source;
            }

            set
            {
                m_Source = value;
            }
        }
        //--------------------------------------------------
        public string StackTrace
        {
            get
            {
                return m_StackTrace;
            }
        }
        //--------------------------------------------------
        public string Message
        {
            get
            {
                return m_Message;
            }

            set
            {
                m_Message = value;
            }
        }
        //--------------------------------------------------
        public DateTime LogDateTime
        {
            get
            {
                return m_LogDateTime;
            }
        }
        //--------------------------------------------------
        public string FormattedLogDateWithDotSeparator
        {
            get
            {
                return m_FormattedLogDateWithDotSeparator;
            }
        }
        //--------------------------------------------------
        public string FormattedLogDateWithDashSeparator
        {
            get
            {
                return m_FormattedLogDateWithDashSeparator;
            }
        }
        //--------------------------------------------------
        public string FormattedLogTimeWithColonSeparator
        {
            get
            {
                return m_FormattedLogTimeWithColonSeparator;
            }
        }
        //--------------------------------------------------
        public string DeveloperComment
        {
            get
            {
                return m_DeveloperComment;
            }
        }
        //--------------------------------------------------
        public LogData SetExceptionData(Exception ex)
        {
            m_IsError = true;
            m_ExceptionType = ex.GetType();
            m_Source = ex.Source;
            m_StackTrace = ex.StackTrace.Trim();
            m_Message = CreateMessagesWithInnerException(ex);

            return this;
        }
        //--------------------------------------------------
        public LogData SetDeveloperComment(string _DeveloperComment)
        {
            m_DeveloperComment = _DeveloperComment;

            return this;
        }
        //--------------------------------------------------
        private string CreateMessagesWithInnerException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return ex.Message + "\r\n\r\n" + CreateMessagesWithInnerException(ex.InnerException);
        }
        //--------------------------------------------------
        private void FormatDate(DateTime _FormattedDateTime)
        {
            string _Result = "";

            string _FormatDateTimeDay = _FormattedDateTime.Day.ToString();

            if (_FormatDateTimeDay.Length == 1)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeDay + ".";

            string _FormatDateTimeMonth = _FormattedDateTime.Month.ToString();

            if (_FormatDateTimeMonth.Length == 1)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeMonth + ".";

            string _FormatDateTimeYear = _FormattedDateTime.Year.ToString();

            if (_FormatDateTimeYear.Length == 1)
            {
                _Result += "000";
            }
            else if (_FormatDateTimeYear.Length == 2)
            {
                _Result += "00";
            }
            else if (_FormatDateTimeYear.Length == 3)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeYear;

            m_FormattedLogDateWithDotSeparator = _Result;

            m_FormattedLogDateWithDashSeparator = _Result.Replace(".", "-");
        }
        //--------------------------------------------------
        public void FormatTime(DateTime _FormattedDateTime)
        {
            string _Result = "";

            string _FormatDateTimeHour = _FormattedDateTime.Hour.ToString();

            if (_FormatDateTimeHour.Length == 1)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeHour + ":";

            string _FormatDateTimeMinute = _FormattedDateTime.Minute.ToString();

            if (_FormatDateTimeMinute.Length == 1)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeMinute + ":";

            string _FormatDateTimeSecond = _FormattedDateTime.Second.ToString();

            if (_FormatDateTimeSecond.Length == 1)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeSecond + ":";

            string _FormatDateTimeMillisecond = _FormattedDateTime.Millisecond.ToString();

            if (_FormatDateTimeMillisecond.Length == 1)
            {
                _Result += "00";
            }
            else if (_FormatDateTimeMillisecond.Length == 2)
            {
                _Result += "0";
            }

            _Result += _FormatDateTimeMillisecond;

            m_FormattedLogTimeWithColonSeparator = _Result;
        }
        //--------------------------------------------------
    }
}
