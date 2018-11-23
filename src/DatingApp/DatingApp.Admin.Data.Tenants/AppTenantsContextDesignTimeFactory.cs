using AspNetCore.ApiBase.Data;
using DatingApp.Data.Tenants;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DatingApp.Data.Tenants
{
    public class TenantsContextContextDesignTimeFactory : DesignTimeDbContextFactoryBase<AppTenantsContext>
    {
        public TenantsContextContextDesignTimeFactory()
            : base("DefaultConnection", typeof(AppTenantsContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override AppTenantsContext CreateNewInstance(DbContextOptions<AppTenantsContext> options)
        {
            return new AppTenantsContext(options);
        }
    }
}
