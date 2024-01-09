using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    /// <summary>
    /// Обращения
    /// </summary>
    public class Appeal
    {
        public long id{ get; set; }
        public long? userId{ get; set; }
        public int? statusId{ get; set; }
        public bool read{ get; set; }
        public long? amount{ get; set; }
        public string details{ get; set; }
        public string[] tags{ get; set; }
        public Messenger socialAccount{ get; set; }
        public int? responseTime{ get; set; }
        public string createdAt{ get; set; }
        public Customer customer{ get; set; }
        public LastMessageForAppeals message{ get; set; }
        public string address{ get; set; }
        public string ttn{ get; set; }
        public string customData{ get; set; }
        public string customFields{ get; set; }
        public int? paymentTypeId{ get; set; }
    }
    public class LastMessageForAppeals
    {
        public int? unread{ get; set; }
        public string timestamp{ get; set; }
        public bool incoming{ get; set; }
        public DataForLastMessage data{ get; set; }
    }
    public class DataForLastMessage
    {
        public string text{ get; set; }
        public string url{ get; set; }
        public AttachmentForMessage[] Attachments{ get; set; }
    }
}
