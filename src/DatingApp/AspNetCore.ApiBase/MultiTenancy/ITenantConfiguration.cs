using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantConfiguration
    {
        string TenantId { get; }
        void ConfigureServices(ConfigurationActionBuilder services, IConfiguration configuration, IHostingEnvironment hostingEnvironment);

        void Configure(IConfiguration configuration);
    }
}
