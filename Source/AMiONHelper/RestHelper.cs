using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class RestHelper
    {
        /// <summary>
        /// Method to get data based on input url
        /// </summary>
        /// <param name="url">fetch data based on the uri</param>
        /// <returns></returns>
        public static async Task<Stream> GetDataFromUrl(string url, string token = "")
        {

            using (HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromMinutes(15)
            })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                using (HttpRequestMessage request = new HttpRequestMessage
                {
                    RequestUri = httpClient.BaseAddress,
                    Method = HttpMethod.Get,
                })
                {
                    // Send Http request
                    HttpResponseMessage httpWebResponse = httpClient.SendAsync(request).GetAwaiter().GetResult();

                    // Parse response
                    if (httpWebResponse.Content != null)
                    {
                        return await httpWebResponse.Content.ReadAsStreamAsync();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Post method to pass the content based on input string and user token
        /// </summary>
        /// <param name="url">Api url of the exposed web service</param>
        /// <param name="token">user token value</param>
        /// <param name="dataTobePassedInRequest">Content of the data which needs to be passed</param>
        /// <returns></returns>
        public static HttpResponseMessage PostData(
            string url, string token, StringContent dataTobePassedInRequest)
        {
            if (dataTobePassedInRequest == null)
            {
                throw new ArgumentNullException(nameof(dataTobePassedInRequest));
            }

            using (HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromMinutes(15)
            })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return httpClient.PostAsync(httpClient.BaseAddress, dataTobePassedInRequest).Result;
            }
        }

        public static async Task<HttpResponseMessage> AsyncDeleteData(string url, string token)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException();
            }

            using (HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromMinutes(15)
            })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await httpClient.DeleteAsync(url);
            }
        }

        public static string SendAsyncRequest(string url, HttpMethod httpMethod, string token ="", StringContent jsonStringContent =null, bool isJsonContentTypeRequired=false)
        {
            if (httpMethod == null)
            {
                throw new ArgumentNullException(nameof(httpMethod));
            }

            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(url), Timeout = TimeSpan.FromMinutes(15) })
            {
                using (var request = new HttpRequestMessage { RequestUri = client.BaseAddress, Method = httpMethod })
                {
                    if (isJsonContentTypeRequired)
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token))
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    if (jsonStringContent != null)
                        request.Content = jsonStringContent;

                    var httpWebResponse = client.SendAsync(request).GetAwaiter().GetResult();

                    // Parse response
                    if (httpWebResponse.IsSuccessStatusCode)
                        return httpWebResponse.Content != null ? httpWebResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult() : string.Empty;

                    var exception = new Exception(Constant.SendAsyncRequestFailedMessage + url+ httpMethod.ToString());
                    exception.Data.Add("StatusCode", httpWebResponse.StatusCode.ToString());
                    exception.Data.Add("ResponseData", httpWebResponse.Content != null ? httpWebResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult() : string.Empty);
                    throw exception;
                }
            }
        }

        //private string PostBatchRequest(string accessToken, string jsonStringContent)
        //{
        //    using (HttpClient client = new HttpClient() { BaseAddress = new Uri($"https://graph.microsoft.com/v1.0/$batch"), Timeout = TimeSpan.FromMinutes(10) })
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //        var request = new HttpRequestMessage { RequestUri = client.BaseAddress, Method = HttpMethod.Post };
        //        request.Content = new StringContent(jsonStringContent, System.Text.Encoding.UTF8, "application/json");
        //        var httpWebResponse = client.SendAsync(request).GetAwaiter().GetResult();

        //        // Parse response
        //        if (httpWebResponse.IsSuccessStatusCode)
        //            return httpWebResponse.Content != null ? httpWebResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult() : string.Empty;

        //        var exception = new Exception("Failed to process the batch request from Shift Graph API");
        //        exception.Data.Add("StatusCode", httpWebResponse.StatusCode.ToString());
        //        exception.Data.Add("ResponseData", httpWebResponse.Content != null ? httpWebResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult() : string.Empty);
        //        throw exception;
        //    }
        //}

    }
}
