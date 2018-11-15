using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.MultiTenancy.Request
{
    public class TenantRequestIdentification
    {
        internal readonly IServiceCollection _services;

        internal TenantRequestIdentification(IServiceCollection services)
        {
            this._services = services;
        }
    }
}
