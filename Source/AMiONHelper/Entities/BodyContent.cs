using Newtonsoft.Json;

namespace AMiON.Helper.Entities
{
    public class BodyContent
    {
         [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
    }
}