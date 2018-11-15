using AspNetCore.ApiBase.MultiTenancy.Data.Tenant.IdentificationStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant
{
    public static class DbContextIdentificationStrategyExtensions
    {
        public static IServiceCollection DifferentConnectionForTenant<TTenant>(this TenantDbContextIdentification identification)
            where TTenant : AppTenant
        {
            return identification._services.AddScoped<IDbContextTenantStrategy, DifferentConnectionTenantDbContext<TTenant>>();
        }

        public static IServiceCollection DifferentSchemaForTenant(this TenantDbContextIdentification identification)
        {
            return identification._services.AddScoped<IDbContextTenantStrategy, DifferentSchemaTenantDbContext>();
        }

        public static IServiceCollection FilterByTenant(this TenantDbContextIdentification identification)
        {
            return identification._services.AddScoped<IDbContextTenantStrategy, FilterTenantDbContext>();
        }

        public static IServiceCollection Dummy(this TenantDbContextIdentification identification)
        {
            return identification._services.AddScoped<IDbContextTenantStrategy, DummyTenantDbContext>();
        }
    }
}
