using AspnetCore.ApiBase.Data.Initializers;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Admin.Data.Identity.Initializers
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
