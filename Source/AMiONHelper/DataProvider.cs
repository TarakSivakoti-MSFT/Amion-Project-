using System;
using System.IO;

namespace AMiON.Helper
{
    static class DataProvider
    {

        /// <summary>
        /// Method to get all teams from Microsoft O365 (Graph API)
        /// </summary>
        /// <param name="token">User token for authentication. Mandatory param</param>
        /// <param name="getPartUrl">Graph API Uri format to fetch the data based on passed part url</param>
        /// <returns></returns>
        public static (bool isApiCallSuccess, string strResponseData) GetDataFromInputUrl(string token, string getPartUrl)
        {
            if (string.IsNullOrEmpty(token))
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException(Constant.TokenEmptyErrorMessage, nameof(token));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            var isApiCallSuccess = false;
            string strResponseData = string.Empty;
            //Send request to graph api to fetch joined teams
            using (var response = RestHelper.GetDataFromUrl(Constant.GraphUrl + getPartUrl, token: token))
            {
                if (response.Result != null)
                {
                    using (StreamReader sr = new StreamReader(response.GetAwaiter().GetResult()))
                    {
                        strResponseData = sr.ReadToEnd();
                        if (!string.IsNullOrEmpty(strResponseData))
                        {
                            isApiCallSuccess = true;
                        }
                    }
                }
                else
                {
                    strResponseData = "error: something went wrong, network error or code error";
                    //TODO: Log response result is empty
                }
            }

            return (isApiCallSuccess, strResponseData);
        }
        #region commented method
        //public static async Task<string> GetUserAccessTokenAsync()
        //{
        //    AuthenticationContext authenticationContext = new AuthenticationContext(Authority, false);
        //    try
        //    {
        //        if (authenticationResult == null)
        //        {
        //            authenticationResult = await authenticationContext
        //                .AcquireTokenAsync(GraphApiUrl, AuthClientID, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.SelectAccount))
        //                .ConfigureAwait(false);
        //        }
        //        else
        //        {
        //            // Use PromptBehavior.Auto for subsequent requests
        //            authenticationResult = await authenticationContext
        //                .AcquireTokenAsync(GraphApiUrl, AuthClientID, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.Auto))
        //                .ConfigureAwait(false);
        //        }
        //        return authenticationResult.AccessToken;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        #endregion
    }
}