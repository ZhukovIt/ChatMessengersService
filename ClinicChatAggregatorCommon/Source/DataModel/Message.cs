using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMed.Clinic.DataModel
{
    public class Message
    {
        public string Id { get; set; }

        public long SendDateTime { get; set; }

        public MessageType MessageType { get; set; }

        public string Text { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        public ChatDialog ChatDialog { get; set; }

        public string GUID { get; set; }

        public string URL { get; set; }
    }

    public enum MessageType
    {
        /// <summary>
        /// Входящее
        /// </summary>
        Incoming,
        /// <summary>
        /// Исходящее
        /// </summary>
        Outgoing
    }
}
