using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Data.UnitOfWork;
using DatingApp.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data.Identity
{
    public class IdentityContext : DbContextIdentityBase<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured == false)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Used for Migrations and when EnsureCreated is called.
            var adminRole = new IdentityRole() { Id = "dd74ef06-e393-4fca-a54a-af90f4b3b274", Name = "admin" };
            adminRole.NormalizedName = adminRole.Name.ToUpper();

            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User { Id = "a0cc60d8-3cc5-4a54-8454-afd6cfd62f32", UserName = "admin", Name = "name", Email = "admin@gmail.com", EmailConfirmed = true, LockoutEnabled = true, SecurityStamp = "InitialSecurityStamp" };
            adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
            adminUser.NormalizedEmail = adminUser.Email.ToUpper();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "password");

            var adminUserRole = new IdentityUserRole<string>() { UserId = adminUser.Id, RoleId = adminRole.Id };

            var adminRoleClaim = new IdentityRoleClaim<string>() { Id= 1, ClaimType = "scope", ClaimValue = ApiScopes.Full, RoleId = adminRole.Id };

            builder.Entity<IdentityRole>().HasData(
              adminRole
              );

            builder.Entity<User>().HasData(
                adminUser
                );

            builder.Entity<IdentityUserRole<string>>().HasData(
               adminUserRole
               );

            builder.Entity<IdentityRoleClaim<string>>().HasData(
              adminRoleClaim
              );
        }

        public override void BuildQueries(ModelBuilder builder)
        {
         
        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}