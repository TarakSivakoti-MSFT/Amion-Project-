using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper.CosmosDB
{
    public interface IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        Guid Id { get; set; }

        [JsonProperty(PropertyName = "CreationDate")]
        //[JsonConverter(typeof(EpochDateTimeConverter))]
        DateTime? CreationDate { get; set; }

        [JsonProperty(PropertyName = "UpdateDate")]
        //[JsonConverter(typeof(EpochDateTimeConverter))]
        DateTime? UpdateDate { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        string ETag { get; set; }
    }
}
