using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace AMiON.Helper.ConfigurationManagerHelper
{
    public static class KeyVaultHelper
    {
        public static string GetSecretValue(string secretUrl)
        {
            using (KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)))
                return keyVaultClient.GetSecretAsync(secretUrl).GetAwaiter().GetResult().Value;
        }
    }
}
