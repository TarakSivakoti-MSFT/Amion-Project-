using AMiON.Helper.CosmosDB;
using System;

namespace AMiON.Helper.Entities
{
    public class AdminTabConfigurationDetails : IEntityDAO
    {
        public AdminTabConfigurationDetails()
        {
            Id = Guid.NewGuid();
        }
        public Guid TenantId { get; set; }
       // public Guid? TeamId { get; set; }
        public string InternalTeamId { get; set; }
        public string ChannelId { get; set; }
        public string Login { get; set; }
        //public string MappingFilePath { get; set; }
        public string[] ExcludedColumns { get; set; }
        public string[] DefaultColumnOrder { get; set; }
        //public string[] SelectedDepartments { get; set; }
        public Guid Id { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string ETag { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public Guid UserId { get; set; }
    }

}
