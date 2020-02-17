using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper.Entities
{
    public class TeamsChatMessage
    {
        //[JsonProperty(PropertyName = "id")]
        //public string Id { get; set; }

        //[JsonProperty(PropertyName = "replyToId")]
        //public string ReplyToId { get; set; }

        //[JsonProperty(PropertyName = "messageType")]
        //public string MessageType { get; set; }

        //[JsonProperty(PropertyName = "subject")]
        //public string Subject { get; set; }

        [JsonProperty(PropertyName = "body")]
        public BodyContent BodyContent { get; set; }

        //[JsonProperty(PropertyName = "attachments")]
        //public ChatMessageAttachment[] ChatMessageAttachment { get; set; }
    }
}
