using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    
    public class Message
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long datetime { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Messenger sa { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool incoming { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MessageItem message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ReplyMessage replyTo { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Author sender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DescriptionPost preview { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Chat source { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string messageId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customId { get; set; }
    }
    public class MessageItem
    {
        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AttachmentForMessage[] attachments { get; set; }

    }
    public class AttachmentForMessage
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Media media { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string preview { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float filesize { get; set; }
    }
    public class Media
    {
        
    }
    public class MediaVk : Media
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }
    }
    public class MediaTelegram : Media
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int parts { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fileId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mime{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string filename{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string md5{ get; set; }
    }
    public class MediaWhatsapp : Media
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string filename{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string src{ get; set; }
    }
    public class MediaInstagramViberOK : Media
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mime{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string size{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type{ get; set; }
    }
    public class MediaOther : Media
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mime{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string path{ get; set; }
    }
    
    public class ReplyMessage 
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long datetime { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int sa { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool incoming { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MessageItem message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Author sender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DescriptionPost preview { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Chat source { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string messageId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customId { get; set; }
    }
    public class DescriptionPost
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string description{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string owner{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string photo{ get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url{ get; set; }
    }
}
