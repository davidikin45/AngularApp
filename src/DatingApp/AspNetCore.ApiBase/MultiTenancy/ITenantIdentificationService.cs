using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantIdentificationService<TTenant> : ITenantIdentificationStrategy
        where TTenant : AppTenant

    {
        Task<TTenant> GetTenantAsync(HttpContext httpContext);
    }
}
