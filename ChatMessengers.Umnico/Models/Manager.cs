using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class Manager : Author
    {
        public string role{ get; set; }
        public bool confirmed{ get; set; }
        public bool allowAllDeals{ get; set; }
        public List<AvailableMessenger> sources{ get; set; }

    }
}
