using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public sealed class DynamicTenantIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
        where TTenant : AppTenant
    {
        private readonly Func<HttpContext, TTenant> _currentTenant;
        private readonly Func<IEnumerable<TTenant>> _allTenants;

        public DynamicTenantIdentificationService(Func<HttpContext, TTenant> currentTenant, Func<IEnumerable<TTenant>> allTenants)
        {
            if (currentTenant == null)
            {
                throw new ArgumentNullException(nameof(currentTenant));
            }

            if (allTenants == null)
            {
                throw new ArgumentNullException(nameof(allTenants));
            }

            this._currentTenant = currentTenant;
            this._allTenants = allTenants;
        }

        public IEnumerable<TTenant> GetAllTenants()
        {
            return this._allTenants();
        }

        public TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context)
        {
            return this._currentTenant(httpContext);
        }
    }
}
