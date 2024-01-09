using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChatMessengersService.FillSiMedDB
{
    /// <summary>
    /// Данный абстрактный класс и его наследники основаны 
    /// на паттерне проектирования Шаблонный метод, так как
    /// подклассы должны будут переопределить этапы алгоритма
    /// метода FillComponent() без изменения его структуры
    /// </summary>
    public abstract class AbstractFillerSiMedDB
    {
        protected DBLogger m_DBLogger;
        protected DataTable m_DataTable;
        protected bool m_IsNewRow;
        //-----------------------------------------------------
        public AbstractFillerSiMedDB(DBLogger _DBLogger)
        {
            m_DBLogger = _DBLogger;
        }
        //-----------------------------------------------------
        /// <summary>
        /// Данный метод определяет алгоритм, согласно которому производные классы
        /// будут заполнять данные в БД
        /// </summary>
        public void FillComponent()
        {
            if (CheckCorrectHandleRequest())
            {
                SetDataTable();
                SetIsNewRow();
                if (m_IsNewRow)
                {
                    CreateNewRow();
                }
                FillDataInRow();
            }
        }
        //-----------------------------------------------------
        #region Защищённые методы, которые переопределят подклассы
        //-----------------------------------------------------
        /// <summary>
        /// Данный абстрактный метод определяет поле <see cref="m_DataTable"/>
        /// </summary>
        protected abstract void SetDataTable();
        //-----------------------------------------------------
        /// <summary>
        /// Данный абстрактный метод определяет является ли строка в таблице
        /// <see cref="m_DataTable"/> новой и записывает результат в поле <see cref="m_IsNewRow"/>
        /// </summary>
        protected abstract void SetIsNewRow();
        //-----------------------------------------------------
        /// <summary>
        /// Данный абстрактный метод создаёт новую строку в таблице <see cref="m_DataTable"/>
        /// </summary>
        protected abstract void CreateNewRow();
        //-----------------------------------------------------
        /// <summary>
        /// Данный абстрактный метод заполняет данными строку в таблице <see cref="m_DataTable"/>
        /// </summary>
        protected abstract void FillDataInRow();
        //-----------------------------------------------------
        /// <summary>
        /// Данный виртуальный метод определяет есть ли смысл заполнять данными
        /// какую-либо строку в таблице <see cref="m_DataTable"/>
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckCorrectHandleRequest()
        {
            return true;
        }
        //-----------------------------------------------------
        #endregion
        //-----------------------------------------------------
        #region Вспомогательные защищённые методы и атрибуты
        //-----------------------------------------------------
        /// <summary>
        /// Данный метод возвращает MESSENGER_TYPE_ID для параметра
        /// перечисления <see cref="MessengersType"/>
        /// </summary>
        /// <param name="_MessengersType"></param>
        /// <returns></returns>
        protected int GetMessengerTypeId(MessengersType _MessengersType)
        {
            return m_DBLogger.MESSENGER_TYPEDataTable
                .Single(r => r.MESSENGER_TYPE_NAME == GetMessengerTypeNameFromMessengersType(_MessengersType)).MESSENGER_TYPE_ID;
        }
        //-----------------------------------------------------
        /// <summary>
        /// Данный метод возвращает MESSENGER_TYPE_NAME для параметра
        /// перечисления <see cref="MessengersType"/>
        /// </summary>
        /// <param name="_MessengersType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected string GetMessengerTypeNameFromMessengersType(MessengersType _MessengersType)
        {
            switch (_MessengersType)
            {
                case MessengersType.WhatsApp:
                    return "WhatsApp";
                case MessengersType.Telegram:
                    return "Telegram";
                case MessengersType.Viber:
                    return "Viber";
                case MessengersType.Vk:
                    return "Вконтакте";
                case MessengersType.Ok:
                    return "Одноклассники";
                default:
                    throw new NotImplementedException($"В таблице MESSENGER_TYPE не нашлось подходящей строки для MessengersType = {_MessengersType}");
            }
        }
        //-----------------------------------------------------
        /// <summary>
        /// Данный метод возвращает MES_DIAL_TYPE_ID для диалога MESSENGER_DIALOG на
        /// основе перечисления <see cref="DialogType"/>
        /// </summary>
        /// <param name="_DialogType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected int GetMessengerDialogTypeIdFromDialogType(DialogType _DialogType)
        {
            switch (_DialogType)
            {
                case DialogType.Message:
                    return 1;
                case DialogType.Comment:
                    return 2;
                default:
                    throw new NotImplementedException($"В таблице MESSENGER_DIALOG_TYPE не нашлось подходящей строки для DialogType = {_DialogType}");
            }
        }
        //-----------------------------------------------------
        /// <summary>
        /// Данный метод возвращает MES_DIAL_UID для диалога MESSENGER_DIALOG
        /// в корректном виде (без спец. символа \n), так как UID диалога от Umnico 
        /// было решено хранить в БД на основе шаблона: 'UID обращения' + '\n' + 'UID канала общения'
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        protected string GetCorrectChatDialogIdFromLogin(string login)
        {
            if (!login.Contains('\n'))
            {
                return login;
            }

            string[] temp = login.Split('\n');
            return temp[0] + " " + temp[1];
        }
        //-----------------------------------------------------
        protected bool CheckEqualPhoneNumber(string _First, string _Second)
        {
            string _FirstPhone = GetCorrectPhoneNumber(new string(_First.Where(c => char.IsDigit(c) || c == '+').ToArray()));
            string _SecondPhone = GetCorrectPhoneNumber(new string(_Second.Where(c => char.IsDigit(c) || c == '+').ToArray()));

            return !string.IsNullOrEmpty(_FirstPhone) && !string.IsNullOrEmpty(_SecondPhone) && _FirstPhone == _SecondPhone;            
        }
        //-----------------------------------------------------
        private string GetCorrectPhoneNumber(string _Phone)
        {
            switch (_Phone.Length)
            {
                case 10:
                    return _Phone;
                case 11:
                    if (_Phone.StartsWith("7") || _Phone.StartsWith("8")) return _Phone.Substring(1);
                    break;
                case 12:
                    if (_Phone.StartsWith("+7")) return _Phone.Substring(2);
                    break;
            }

            return null;
        }
        //-----------------------------------------------------
        #endregion
        //-----------------------------------------------------
    }
}
