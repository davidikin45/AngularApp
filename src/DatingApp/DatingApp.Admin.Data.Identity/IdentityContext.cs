using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using DatingApp.Admin.Domain;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Admin.Data.Identity
{
    public class IdentityContext : DbContextIdentityTentantBase<User>
    {
    
        public IdentityContext(DbContextOptions<IdentityContext> options = null, ITenantService tenantService = null) : base(options, tenantService)
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