using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class MessagesHistory
    {
        public int cursor{ get; set; }
        public int source{ get; set; }
        public List<Message> messages{ get; set; }
    }
}
