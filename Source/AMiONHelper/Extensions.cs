using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace AMiON.Helper
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
        {
            return new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Generic method to deserialize the 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T AsObject<T>(this object stringObject)
        {
            if (stringObject == null)
            {
                throw new System.ArgumentNullException(nameof(stringObject));
            }

            return JsonConvert.DeserializeObject<T>(stringObject.ToString());
        }

        /// <summary>
        /// Method to parse JSON string to dynamic JObject
        /// </summary>
        /// <param name="jsonStringObject"></param>
        /// <returns></returns>
        public static JObject ParseJson(this object jsonStringObject)
        {
            if (jsonStringObject == null)
            {
                throw new System.ArgumentNullException(nameof(jsonStringObject));
            }

            return JObject.Parse(json: jsonStringObject.ToString());
        }
    }
}
