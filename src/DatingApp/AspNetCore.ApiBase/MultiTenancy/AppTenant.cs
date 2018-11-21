using AspNetCore.ApiBase.Domain;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public class AppTenant : EntityBase<string>, IEntitySoftDelete
    {
        public string Name { get; set; }

        public string[] RequestIpAddresses { get; set; }
        public string[] HostNames { get; set; }

        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();

        public string GetConnectionString(string name)
        {
            if(ConnectionStrings.ContainsKey(name))
            {
                return ConnectionStrings[name];
            }

            return null;
        }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    }
}
