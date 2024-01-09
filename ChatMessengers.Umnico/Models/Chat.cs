using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    /// <summary>
    /// Канал
    /// </summary>
    public class Chat
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id{ get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int realId{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type{ get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int saId{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sender{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string token{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string identifier{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? expires{ get; set; }
    }
}
