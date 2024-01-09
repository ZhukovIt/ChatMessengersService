using BulkMessagesWebServer;
using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class MassMessagesCreatorObjects
    {
        public MessengerDialogDBContent CreateMessengerDialogDBContent(int _PersonId, Tuple<int, string> _TelegramSourceTypeData, 
            Tuple<int, string> _WhatsAppSourceTypeData, string _PersonPhoneNumber)
        {
            MessengerDialogDBContent _Result = new MessengerDialogDBContent(_PersonId);

            if (_TelegramSourceTypeData?.Item2 != null)
            {
                _Result.TelegramSourceTypeId = _TelegramSourceTypeData.Item1;
                _Result.TelegramSourceTypeUId = _TelegramSourceTypeData.Item2;
            }

            if (_WhatsAppSourceTypeData?.Item2 != null)
            {
                _Result.WhatsAppSourceTypeId = _WhatsAppSourceTypeData.Item1;
                _Result.WhatsAppSourceTypeUId = _WhatsAppSourceTypeData.Item2;
            }

            string _PhoneNumber = MassMessagesService.GetCorrectPhoneNumber(_PersonPhoneNumber);

            if (_PhoneNumber != null)
            {
                _Result.PersonPhoneNumber = _PhoneNumber;
            }

            return _Result;
        }
        //----------------------------------------------------------------------------------------------
        public DeciderCreateDBObjects DecideToCreateDBObjects(MessengerDialogCreateContent _Content)
        {
            DeciderCreateDBObjects _Result = new DeciderCreateDBObjects();

            _Result.CanAddPersonMessengerType = _Content.NeedCreateMessengerDialog &&
                _Content.PersonMessengerTypeId <= 0;

            _Result.CanAddMessengerDialog = _Content.NeedCreateMessengerDialog;

            _Result.CanSetPersonMessengerTypeIdByMessengerDialogId = !_Content.NeedCreateMessengerDialog &&
                _Content.MessengerDialogHasNotPersonMessengerTypeId;

            _Result.CanAddMessengerDialogMessageAttachment = _Content.NeedCreateMessengerDialogMessageAttachment;

            return _Result;
        }
        //----------------------------------------------------------------------------------------------
    }
}
