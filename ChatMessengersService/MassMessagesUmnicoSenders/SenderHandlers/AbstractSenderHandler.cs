using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers
{
    public class SenderHandleRequestException : Exception
    {
        public SenderHandleRequestException(string _Message) : base(_Message) { }
    }
    //------------------------------------------------------------------
    public abstract class AbstractSenderHandler
    {
        private bool m_CanSendMessage = true;
        private string m_SendMessageError;
        protected AbstractSenderHandler m_Successor;
        protected string m_TypeDefinition;
        //------------------------------------------------------------------
        public bool CanSendMessage
        {
            get => GetSenderWhichHasErrorMessage()?.m_CanSendMessage ?? true;
        }
        //------------------------------------------------------------------
        public string TypeDefinition
        {
            get => GetSenderWhichHasErrorMessage()?.m_TypeDefinition ?? "<Данные отсутствуют>";
        }
        //------------------------------------------------------------------
        public string SendMessageError
        {
            get => GetSenderWhichHasErrorMessage()?.m_SendMessageError;

            protected set
            {
                m_SendMessageError = value;

                m_CanSendMessage = value == null;

                m_TypeDefinition = Definition ?? "<Данные отсутствуют>";
            }
        }
        //------------------------------------------------------------------
        public AbstractSenderHandler Successor
        {
            get => m_Successor;
            set => m_Successor = value;
        }
        //------------------------------------------------------------------
        public abstract MessengerSendAction HandleRequest(MessengerDialogDBContent _Content);
        //------------------------------------------------------------------
        protected abstract string Definition { get; }
        //------------------------------------------------------------------
        public void ClearErrorData()
        {
            if (m_Successor != null)
                m_Successor.ClearErrorData();
            SendMessageError = null;
        }
        //------------------------------------------------------------------
        private AbstractSenderHandler GetSenderWhichHasErrorMessage()
        {
            if (m_SendMessageError != null)
                return this;
            if (m_Successor != null)
                return m_Successor.GetSenderWhichHasErrorMessage();
            return null;
        }
        //------------------------------------------------------------------
    }
}
