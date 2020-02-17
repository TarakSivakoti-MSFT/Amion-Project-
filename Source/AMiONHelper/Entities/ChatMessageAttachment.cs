using Newtonsoft.Json;

namespace AMiON.Helper.Entities
{
    public class ChatMessageAttachment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }
}