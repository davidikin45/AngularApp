using AspNetCore.ApiBase.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant
{
    public abstract class DbContextIdentityTentantBase<TUser> : DbContextIdentityBase<TUser> where TUser : IdentityUser
    {
        public DbContextIdentityTentantBase(DbContextOptions options)
            :base(options)
        {

        }

        private readonly ITenantService _tenantService;

        public DbContextIdentityTentantBase(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _tenantService?.TenantStrategy?.OnModelCreating(modelBuilder, this, _tenantService.GetTenant(), _tenantService.TenantPropertyName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            _tenantService?.TenantStrategy?.OnConfiguring(optionsBuilder, _tenantService.GetTenant(), _tenantService.TenantPropertyName);
        }

        #region Save Changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            _tenantService?.TenantStrategy?.OnSaveChanges(this, _tenantService.GetTenant(), _tenantService.TenantPropertyName);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            _tenantService?.TenantStrategy?.OnSaveChanges(this, _tenantService.GetTenant(), _tenantService.TenantPropertyName);

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _tenantService?.TenantStrategy?.OnSaveChanges(this, _tenantService.GetTenant(), _tenantService.TenantPropertyName);

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken
            = default(CancellationToken))
        {
            _tenantService?.TenantStrategy?.OnSaveChanges(this, _tenantService.GetTenant(), _tenantService.TenantPropertyName);

            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}
