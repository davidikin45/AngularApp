using AspnetCore.ApiBase.Data.Initializers;
using DatingApp.Tenant.Domain;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Data.Tenant.Identity.Initializers
{
    public class IdentityContextInitializerDropMigrate : ContextInitializerDropMigrate<IdentityContext>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        public IdentityContextInitializerDropMigrate(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public IdentityContextInitializerDropMigrate()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public override void Seed(IdentityContext context, string tenantId)
        {
            var dbSeeder = new DbSeed(_passwordHasher);
            dbSeeder.SeedData(context);
        }
    }
}
