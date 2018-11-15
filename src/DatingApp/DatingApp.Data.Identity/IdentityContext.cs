using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.MultiTenancy;
using DatingApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data.Identity
{
    public class IdentityContext : DbContextIdentityBase<User>
    {
      
        public static IdentityContext Create(DbContextOptions<IdentityContext> options)
        {
            return new IdentityContext(options);
        }

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //https://stackoverflow.com/questions/47767267/ef-core-2-how-to-include-roles-navigation-property-on-identityuser
            builder.Entity<User>()
               .HasMany(e => e.Roles)
               .WithOne()
               .HasForeignKey(e => e.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        }

        public override void BuildQueries(ModelBuilder builder)
        {
         
        }
    }
}