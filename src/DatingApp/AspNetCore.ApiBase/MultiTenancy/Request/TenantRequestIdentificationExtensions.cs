using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy.Request
{
    public static class TenantRequestIdentificationExtensions
    {
        public static IServiceCollection DynamicTenant<TContext,TTenant>(this TenantRequestIdentification<TContext, TTenant> identification, Func<HttpContext, TTenant> currentTenant, Func<IEnumerable<TTenant>> allTenants)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TContext,TTenant>>(sp => new DynamicTenantIdentificationService<TContext,TTenant>(sp.GetRequiredService<IHttpContextAccessor>(), sp.GetRequiredService<ILogger<ITenantIdentificationService<TContext, TTenant>>>(), currentTenant, allTenants));
        }

        public static IServiceCollection TenantForHostQueryStringSourceIP<TContext,TTenant>(this TenantRequestIdentification<TContext, TTenant> identification)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TContext, TTenant>, TenantHostQueryStringRequestIpIdentificationService<TContext, TTenant>>();
        }

        public static IServiceCollection TenantForHost<TContext,TTenant>(this TenantRequestIdentification<TContext, TTenant> identification)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TContext, TTenant>, HostIdentificationService<TContext, TTenant>>();
        }

        public static IServiceCollection TenantForQueryString<TContext,TTenant>(this TenantRequestIdentification<TContext, TTenant> identification)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TContext, TTenant>, QueryStringIdentificationService<TContext, TTenant>>();
        }

        public static IServiceCollection TenantForSourceIP<TContext,TTenant>(this TenantRequestIdentification<TContext, TTenant> identification)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<ITenantIdentificationService<TContext, TTenant>, SourceIPIdentificationService<TContext, TTenant>>();
        }
    }
}
