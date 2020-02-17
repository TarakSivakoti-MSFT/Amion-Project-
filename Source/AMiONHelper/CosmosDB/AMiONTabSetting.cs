using AMiON.Helper.ConfigurationManagerHelper;
using AMiON.Helper.Entities;
using System;

namespace AMiON.Helper.CosmosDB
{

    public class AMiONTabSetting
    {
        public string CosmosDB = AppSetting.CosmosDB;
        public string AdminTabConfigurationDetailCollectionName = AppSetting.AdminTabConfigurationDetailCollectionName;
        public string UserSettingCollection = AppSetting.UserSettingCollectionName;


        public AdminTabConfigurationDetails GetAdminTabConfigurationDetails(string InternalTeamId, string ChannelId) =>
            CosmosDB<AdminTabConfigurationDetails>.GetItemAsync(x => x.InternalTeamId.ToString() == InternalTeamId.ToString() && 
            x.ChannelId.ToString()== ChannelId.ToString(), CosmosDB, AdminTabConfigurationDetailCollectionName);


        public UserSetting GetUserSetting(string UserID, string InternalTeamId) =>
           CosmosDB<UserSetting>.GetItemAsync(x => x.UserId.ToString() == UserID.ToString() && x.InternalTeamId.ToString() == InternalTeamId.ToString(), CosmosDB, UserSettingCollection);

        public string InsertOrUpdateAdminTabConfigurationDetailCollection(AdminTabConfigurationDetails adminTabConfigurationDetails) =>
           CosmosDB<AdminTabConfigurationDetails>.InsertOrUpdateCollection(CosmosDB, AdminTabConfigurationDetailCollectionName, adminTabConfigurationDetails);


        public string InsertOrUpdateUserSetting(UserSetting userSetting) => 
            CosmosDB<UserSetting>.InsertOrUpdateCollection(CosmosDB, UserSettingCollection, userSetting);
        
    }

}
