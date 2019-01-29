
using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Data;
using AspNetCore.ApiBase.Data.UnitOfWork;
using DatingApp.Tenant.Core;
using DatingApp.Tenant.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.Data.Tenant.Identity
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
               ResourceCollectionsCore.Auth.Scopes.Register,
               ResourceCollectionsCore.Auth.Scopes.Authenticate,
               ResourceCollectionsCore.Auth.Scopes.ForgotPassword,
               ResourceCollectionsCore.Auth.Scopes.ResetPassword
               ),
               new SeedRole(Role.authenticated.ToString(),
               ResourceCollections.Values.Scopes.Create,
               ResourceCollections.Values.Scopes.ReadOwner,
               ResourceCollections.Values.Scopes.UpdateOwner,
               ResourceCollections.Values.Scopes.DeleteOwner
               ),
               new SeedRole(Role.read_only.ToString(),
               ResourceCollectionsCore.Admin.Scopes.Read
               ),
               new SeedRole(Role.administrator.ToString(),
               ResourceCollectionsCore.Admin.Scopes.Full
               )
            };
        }

        public override IEnumerable<SeedUser> GetUserRoles(DbContextIdentityBase<User> context)
        {
            return new List<SeedUser>()
            {
                 new SeedUser(User.CreateUser(_passwordHasher, "admin", "admin@admin.com", "password"), true, Role.administrator.ToString()),
                 new SeedUser(User.CreateUser(_passwordHasher, "api", "api@api.com", "password"), true, Role.read_only.ToString())
            };
        }
    }
}
