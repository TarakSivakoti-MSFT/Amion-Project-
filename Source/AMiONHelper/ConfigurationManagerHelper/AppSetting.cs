using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper.ConfigurationManagerHelper
{
    public static class AppSetting
    {
        public static string AuthClientId => KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["AuthClientId"].ToString());
        public static string ColumnNamesConfig => ConfigurationManager.AppSettings["ColumnNamesConfig"].ToString();
        public static string ShowCharacterLength => ConfigurationManager.AppSettings["ShowCharacterLength"].ToString();
        public static string CacheTimeOut => ConfigurationManager.AppSettings["CacheTimeOut"].ToString();
        public static string TenantIdWithApplicationPermissions=> KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["TenantIdWithApplicationPermissions"].ToString());
        public static string ClientIdWithApplicationPermissions => KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["ClientIdWithApplicationPermissions"].ToString());
        public static string ClientSecretWithApplicationPermissions => KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["ClientSecretWithApplicationPermissions"].ToString());
        public static string BlobStorageConnection => KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["BlobStorageConnection"].ToString());
        public static string CosmosDB => ConfigurationManager.AppSettings["CosmosDB"].ToString();
        public static string AdminTabConfigurationDetailCollectionName => ConfigurationManager.AppSettings["AdminTabConfigurationDetailCollectionName"].ToString();
        public static string UserSettingCollectionName => ConfigurationManager.AppSettings["UserSettingCollectionName"].ToString();
        public static string CosmosURL => ConfigurationManager.AppSettings["CosmosURL"].ToString();
        public static string CosmosAuthKey => KeyVaultHelper.GetSecretValue(ConfigurationManager.AppSettings["CosmosAuthKey"].ToString());
    }
}
