using AspNetCore.ApiBase.Domain;

namespace DatingApp.Admin.Domain
{
    public class Value : EntityBase<int>
    {
        public string Name { get; set; }
    }
}