using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class CMUmnicoEnums
    {
        public enum AppealsType
        {
            New,
            Active,
            Archive,
            All
        }
        public enum MessengerTypes
        {
            fb_group, //- группа Facebook
            fb_messenger, //- мессенджер Facebook
            vk_group, //- группа Vkontakte
            telebot, //- Telegram-бот
            whatsapp2, //- Whatsapp
            widget, //- виджет «Umnico»
            telegram, //- Telegram-personal
            waba, //- Whatsapp Business API
            instagramV3, //- Instagram Official
            viber_bot, //- Viber
            ok, //- Одноклассники
            avito, //- Avito
            mailbox, //- Mailbox
            other //- other
        }
        public static SiMed.Clinic.DataModel.MessengersType? GetMessengerTypes(MessengerTypes type)
        {
            switch (type)
            {
                case MessengerTypes.fb_group:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.fb_messenger:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.vk_group:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.Vk;
                        break;
                    }
                case MessengerTypes.telebot:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.whatsapp2:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.WhatsApp;
                        break;
                    }
                case MessengerTypes.widget:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.telegram:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.Telegram;
                        break;
                    }
                case MessengerTypes.waba:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.instagramV3:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.Instagram;
                        break;
                    }
                case MessengerTypes.viber_bot:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.Viber;
                        break;
                    }
                case MessengerTypes.ok:
                    {
                        return SiMed.Clinic.DataModel.MessengersType.Ok;
                        break;
                    }
                case MessengerTypes.avito:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.mailbox:
                    {
                        return null;
                        break;
                    }
                case MessengerTypes.other:
                    {
                        return null;
                        break;
                    }
                    
                default: break;
            }
            return null;
        }
    }
}
