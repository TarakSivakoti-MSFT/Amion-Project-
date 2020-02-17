using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace AMiON.Helper
{
    public class Authentication
    {
        private static readonly string AuthURL = "https://login.microsoftonline.com/common";
        private static readonly string AuthClientId = "9c9869cf-8e00-45d0-9ee8-fed245fee4c0";

        public static string GetToken()
        {
            var authContext = new AuthenticationContext(AuthURL);
            var result = authContext.AcquireTokenAsync(Constant.GraphUrl, AuthClientId, new Uri("https://localhost"), new PlatformParameters(PromptBehavior.SelectAccount)).GetAwaiter().GetResult();
            return result.AccessToken;
        }
    }
}
