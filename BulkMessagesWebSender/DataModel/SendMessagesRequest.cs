using BulkMessagesWebServer.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer
{
    [Serializable]
    public sealed class SendMessagesRequest
    {
        public SendMessagesTypes SendMessagesType
        {
            get
            {
                if (Guid == null)
                {
                    return SendMessagesTypes.ManyPersonalMessagesToManyRecipients;
                }
                else if (PersonsInfo.Count() == 1)
                {
                    return SendMessagesTypes.OneMessageToOneRecipient;
                }
                else
                {
                    return SendMessagesTypes.OneMessageToManyRecipients;
                }
            }
        }
        //---------------------------------------------------------------
        public IEnumerable<PersonInfo> PersonsInfo { get; set; }

        public string Image { get; set; }

        public string Guid { get; set; }
    }
}
