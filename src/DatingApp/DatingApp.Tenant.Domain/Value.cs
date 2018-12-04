using AspNetCore.ApiBase.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DatingApp.Tenant.Domain
{
    public class Value : EntityBase<int>
    {
        public string Name { get; set; }
    }
}