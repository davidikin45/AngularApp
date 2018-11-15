using AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy.Request
{
    public static class TenantRequestIdentificationExtensions
    {
        public static IServiceCollection DynamicTenant<TTenant>(this TenantRequestIdentification identification, Func<HttpContext, TTenant> currentTenant, Func<IEnumerable<TTenant>> allTenants) where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TTenant>>(sp => new DynamicTenantIdentificationService<TTenant>(currentTenant, allTenants));
        }

        public static IServiceCollection TenantForHostQueryStringSourceIP<TTenant>(this TenantRequestIdentification identification) where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TTenant>, TenantHostQueryStringRequestIpIdentificationService<TTenant>>();
        }

        public static IServiceCollection TenantForHost<TTenant>(this TenantRequestIdentification identification) where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TTenant>, HostIdentificationService<TTenant>>();
        }

        public static IServiceCollection TenantForQueryString<TTenant>(this TenantRequestIdentification identification) where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TTenant>, QueryStringIdentificationService<TTenant>>();
        }

        public static IServiceCollection TenantForSourceIP<TTenant>(this TenantRequestIdentification identification) where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TTenant>, SourceIPIdentificationService<TTenant>>();
        }
    }
}
