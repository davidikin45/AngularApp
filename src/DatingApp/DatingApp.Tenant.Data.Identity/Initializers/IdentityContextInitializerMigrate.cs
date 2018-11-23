using AspnetCore.ApiBase.Data.Initializers;
using DatingApp.Tenant.Domain;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Data.Tenant.Identity.Initializers
{
    public class IdentityContextInitializerMigrate : ContextInitializerMigrate<IdentityContext>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        public IdentityContextInitializerMigrate(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public IdentityContextInitializerMigrate()
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
