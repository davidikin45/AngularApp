using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;

namespace DatingApp.Tenant.Api.ApiClient
{
    public class TenantsApiClient : TenantsApiClientBase<AppTenant>
    {
        public TenantsApiClient(HttpClient client)
            : base(client, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })
        {

        }
    }
}
