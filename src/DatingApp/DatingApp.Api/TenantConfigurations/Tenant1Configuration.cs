using AspNetCore.ApiBase.MultiTenancy;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
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

        public void ConfigureServices(ConfigurationActionBuilder services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {

        }
    }
}
