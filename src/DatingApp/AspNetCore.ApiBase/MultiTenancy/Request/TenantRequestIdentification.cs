using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.MultiTenancy.Request
{
    public class TenantRequestIdentification<TTenant>
      where TTenant : AppTenant
    {
        internal readonly IServiceCollection _services;

        internal TenantRequestIdentification(IServiceCollection services)
        {
            this._services = services;
        }
    }
}
