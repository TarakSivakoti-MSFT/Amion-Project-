using AMiON.Helper.ConfigurationManagerHelper;
using Microsoft.Identity.Client;
using System;

namespace AMiON.Helper.AuthToken
{
    public class AuthToken
    {

        public string GetToken()
        {
            var tenantID = AppSetting.TenantIdWithApplicationPermissions;
            var ClientId = AppSetting.ClientIdWithApplicationPermissions;
            var ClientSecret = AppSetting.ClientSecretWithApplicationPermissions;
            var Authority = "https://login.microsoftonline.com/" + tenantID + "/oauth2/v2.0/token";
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            return ConfidentialClientApplicationBuilder.Create(ClientId)
                                                       .WithClientSecret(ClientSecret)
                                                       .WithAuthority(new Uri(Authority))
                                                       .Build().AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
        }
    }
}
