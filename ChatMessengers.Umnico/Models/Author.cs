using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class Author
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string login{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string avatar{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type{ get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int customerId{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string socialId{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string profileUrl{ get; set; }
    }
}
