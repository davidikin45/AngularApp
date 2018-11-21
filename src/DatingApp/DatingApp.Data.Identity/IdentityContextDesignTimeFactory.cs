using AspNetCore.ApiBase.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DatingApp.Data.Identity
{
    public class IdentityContextContextDesignTimeFactory : DesignTimeDbContextFactoryBase<IdentityContext>
    {
        public IdentityContextContextDesignTimeFactory()
            : base("DefaultConnection", typeof(IdentityContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override IdentityContext CreateNewInstance(DbContextOptions<IdentityContext> options)
        {
            return new IdentityContext(options);
        }
    }
}
