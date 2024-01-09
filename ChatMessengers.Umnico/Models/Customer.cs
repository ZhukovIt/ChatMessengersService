using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class Customer : Author
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string email{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phone{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CustomerMessengers[] profiles{ get; set; }
    }
    public class CustomerMessengers
    {
        public int id{ get; set; }
        public string login{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string socialId{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string profileUrl{ get; set; }
    }
}
