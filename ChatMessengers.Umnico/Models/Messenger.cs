using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    /// <summary>
    /// Интеграции
    /// </summary>
    public class Messenger
    {
        public int id{ get; set; }
        public string type{ get; set; }
        public string login{ get; set; }
        public string avatar{ get; set; }
        public string externalId{ get; set; }
        public string status{ get; set; }
        public string identifier{ get; set; }
        public string url{ get; set; }
               
    }
    public class AvailableMessenger
    {
        public int id{ get; set; }
        public Messenger messenger{ get; set; }
    }
}                         
                          
                          