using BulkMessagesWebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers;

namespace BulkMessagesWebServer.Tests.Common
{
    public static class MotherObject
    {
        public static MainOptions CreateMainOptions()
        {
            return new MainOptions("*", 12341).SetGUID("7a93e9a3-ce8b-4b4c-937f-239e33c50b2d");
        }
        //----------------------------------------------------------------------------------------
        public static AbstractSenderHandler CreateMainSenderHandler()
        {
            AbstractSenderHandler _PriorityWhatsAppSenderHandler = new PriorityWhatsAppSenderHandler();
            AbstractSenderHandler _PriorityTelegramSenderHandler = new PriorityTelegramSenderHandler();
            AbstractSenderHandler _PriorityVkontakteSenderHandler = new PriorityVkontakteSenderHandler();
            AbstractSenderHandler _ExistsMessengerSenderHandler = new ExistsMessengerSenderHandler();
            AbstractSenderHandler _TelegramFirstSenderHandler = new TelegramFirstSenderHandler();

            _PriorityWhatsAppSenderHandler.Successor = _PriorityTelegramSenderHandler;
            _PriorityTelegramSenderHandler.Successor = _PriorityVkontakteSenderHandler;
            _PriorityVkontakteSenderHandler.Successor = _ExistsMessengerSenderHandler;
            _ExistsMessengerSenderHandler.Successor = _TelegramFirstSenderHandler;

            return _PriorityWhatsAppSenderHandler;
        }
        //----------------------------------------------------------------------------------------
    }
}
