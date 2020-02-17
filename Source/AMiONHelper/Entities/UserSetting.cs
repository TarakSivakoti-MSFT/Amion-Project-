using AMiON.Helper.CosmosDB;
using System;

namespace AMiON.Helper.Entities
{

    public class UserSetting : IEntityDAO
    {
        public UserSetting()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        //  public Guid TeamId { get; set; }
        public string InternalTeamId { get; set; }
        public string ChannelId { get; set; }
        public Guid UserId { get; set; }
       // public string[] ColumnsOrder { get; set; }

        public string[] ColumnsOrderByKey { get; set; }
        public string[] RemovedColumnsByKey { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string ETag { get; set; }
    }
}
