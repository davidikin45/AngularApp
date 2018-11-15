using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Data;
using AspNetCore.ApiBase.Data.UnitOfWork;
using DatingApp.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.Data.Identity
{
    public class DbSeed : DbSeedIdentity<User>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        public DbSeed(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public override IEnumerable<SeedRole> GetRolePermissions(DbContextIdentityBase<User> context)
        {
            //To allow anonymous need to add method here AND add AllowAnonymousAttribute
            return new List<SeedRole>()
            {
               new SeedRole(Role.anonymous.ToString(),
               ResourceOperationsCore.Auth.Scopes.Register,
               ResourceOperationsCore.Auth.Scopes.Authenticate,
               ResourceOperationsCore.Auth.Scopes.ForgotPassword,
               ResourceOperationsCore.Auth.Scopes.ResetPassword
               ),
               new SeedRole(Role.authenticated.ToString(),
               ResourceOperations.Values.Scopes.Create,
               ResourceOperations.Values.Scopes.ReadOwner,
               ResourceOperations.Values.Scopes.UpdateOwner,
               ResourceOperations.Values.Scopes.DeleteOwner
               ),
               new SeedRole(Role.administrator.ToString(),
               ResourceOperationsCore.Admin.Scopes.Full
               )
            };
        }

        public override IEnumerable<SeedUser> GetUserRoles(DbContextIdentityBase<User> context)
        {
            return new List<SeedUser>()
            {
                 new SeedUser(User.CreateUser(_passwordHasher, "a0cc60d8-3cc5-4a54-8454-afd6cfd62f32", "admin", "admin@admin.com", "password"), true, Role.administrator.ToString())
            };
        }
    }
}
