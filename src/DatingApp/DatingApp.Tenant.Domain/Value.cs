using AspNetCore.ApiBase.Domain;

namespace DatingApp.Tenant.Domain
{
    public class Value : EntityBase<int>
    {
        public string Name { get; set; }
    }
}