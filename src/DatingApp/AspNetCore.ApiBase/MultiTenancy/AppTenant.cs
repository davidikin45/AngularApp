using AspNetCore.ApiBase.Domain;
using System;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public class AppTenant : EntityBase<string>, IEntitySoftDelete
    {
        public string Name { get; set; }

        public string[] RequestIpAddresses { get; set; }
        public string[] HostNames { get; set; }

        public string ConnectionString { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    }
}
