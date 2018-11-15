using AspNetCore.ApiBase.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.Api.TenantConfigurations
{
    public class Tenant1Configuration : ITenantConfiguration
    {
        public string TenantId => "t1";

        public void Configure(IConfiguration configuration)
        {
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
        }
    }
}
