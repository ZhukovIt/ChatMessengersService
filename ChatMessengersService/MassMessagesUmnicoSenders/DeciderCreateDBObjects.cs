using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class DeciderCreateDBObjects
    {
        public bool CanAddPersonMessengerType { get; set; }

        public bool CanAddMessengerDialog { get; set; }

        public bool CanSetPersonMessengerTypeIdByMessengerDialogId { get; set; }

        public bool CanAddMessengerDialogMessageAttachment { get; set; }
    }
}
