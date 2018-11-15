using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantConfiguration
    {
        string TenantId { get; }
        void Configure(IConfiguration configuration);
        void ConfigureServices(IServiceCollection services);
    }
}
