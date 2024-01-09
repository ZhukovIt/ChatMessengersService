using ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class CachedSendingFirstMessage
    {
        public bool Result { get; set; }

        public string ErrorMessage { get; set; }

        public MessengerSendMethods MessengerSendMethod { get; set; }

        public string SourceTypeUId { get; set; }

        public string PhoneNumber { get; set; }

        public MessengersType MessengerType { get; set; }

        public DateTimeOffset TimeToLife { get; set; }
    }
}
