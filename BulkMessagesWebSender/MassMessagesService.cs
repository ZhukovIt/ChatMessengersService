using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer
{
    public static class MassMessagesService
    {
        public static string GetMessageByException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return GetMessageByException(ex.InnerException);
        }
        //--------------------------------------------------------------
        public static bool DateTimeEquals(this DateTime _Self, DateTime _Other)
        {
            if (_Self.Year != _Other.Year)
            {
                return false;
            }

            if (_Self.Month != _Other.Month)
            {
                return false;
            }

            if (_Self.Day != _Other.Day)
            {
                return false;
            }

            if (_Self.Hour != _Other.Hour)
            {
                return false;
            }

            if (_Self.Minute != _Other.Minute)
            {
                return false;
            }

            if (_Self.Second != _Other.Second)
            {
                return false;
            }

            return true;
        }
        //--------------------------------------------------------------
        public static string GetCorrectPhoneNumber(string _PhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(_PhoneNumber))
            {
                return null;
            }

            string _Data = new string(_PhoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());

            if (_Data.Length == 12 && _Data.StartsWith("+7"))
            {
                return _Data.Substring(2, _Data.Length - 2);
            }
            else if (_Data.Contains('+'))
            {
                return null;
            }
            else if (_Data.Length == 11 && _Data.StartsWith("7"))
            {
                return _Data.Substring(1, _Data.Length - 1);
            }
            else if (_Data.Length == 11 && _Data.StartsWith("8"))
            {
                return _Data.Substring(1, _Data.Length - 1);
            }
            else if (_Data.Length == 10)
            {
                return _Data;
            }

            return null;
        }
        //--------------------------------------------------------------
        public static string[] GetSupportedMessengerTypes()
        {
            return new string[]
            {
                "WhatsApp",
                "Telegram",
                "Вконтакте"
            };
        }
        //--------------------------------------------------------------
        public static string GetSupportedMessengerTypesAsString()
        {
            return string.Join(" ", GetSupportedMessengerTypes());
        }
        //--------------------------------------------------------------
    }
}
