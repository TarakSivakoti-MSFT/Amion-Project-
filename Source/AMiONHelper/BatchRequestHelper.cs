using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class BatchRequestHelper
    {
        public static List<string> GenerateJSONBatchRequestList(List<(string guid, string method, string url)> batchDetails, int batchCount = 20)
        {
            var requestList = new List<string>();
            if (batchDetails != null)
            {
                var requestBatchCount = 1;
                var requestBody = new List<string>();
                foreach (var batch in batchDetails)
                {
                    requestBody.Add($"{{\"id\": \"{requestBatchCount + "#" + batch.guid}\",\"method\": \"{batch.method}\",\"url\": \"{batch.url}\"}}");
                    if (requestBatchCount == batchCount)
                    {
                        requestList.Add($"{{\"requests\": [{String.Join(",", requestBody.ToArray())}]}}");
                        requestBatchCount = 0;
                        requestBody.Clear();
                    }
                    requestBatchCount++;
                }
                if (requestBody.Count > 0)
                    requestList.Add($"{{\"requests\": [{String.Join(",", requestBody.ToArray())}]}}");
            }
            return requestList;
        }

        public List<T> ProcessResponseBatch<T>(List<string> responseList, Func<JToken, IEnumerable<T>> parseResponse)
        {
            return responseList.Select(response =>
            {
                var jObject = JObject.Parse(response);

                if (jObject["responses"] != null && jObject["responses"].HasValues)
                {
                    return jObject["responses"].ToList().SelectMany(jToken =>
                    {
                        if (jToken["status"].Value<int>() == (int)HttpStatusCode.OK)
                            return parseResponse(jToken);
                        else
                        {
                            // log the exception
                        }
                        return new List<T>();
                    }).ToList();
                }
                return null;
            }).SelectMany(x => x).ToList();
        }
    }
}
