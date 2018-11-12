using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.UnitOfWork
{
    public abstract class DbContextIdentityBase<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
    {
        public DbContextIdentityBase(DbContextOptions options)
            : base(options)
        {

            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            ChangeTracker.LazyLoadingEnabled = true;
            ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static readonly ILoggerFactory CommandLoggerFactory
        = new LoggerFactory()
        .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name))
        .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

        public static readonly ILoggerFactory ChangeTrackerLoggerFactory
         = new LoggerFactory()
        .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Debug) && (categoryName == DbLoggerCategory.ChangeTracking.Name))
        .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Debug) && (categoryName == DbLoggerCategory.ChangeTracking.Name));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(CommandLoggerFactory).EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.HasDefaultSchema("dbo"); //SQLite doesnt have schemas

            builder.RemovePluralizingTableNameConvention();
            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            BuildQueries(builder);
        }

        public abstract void BuildQueries(ModelBuilder builder);

        #region Seed
        public abstract void Seed();
        #endregion

        #region Migrate
        public void Migrate()
        {
            Database.Migrate();
        }
        #endregion

        #region Timestamps
        private void AddTimestamps()
        {
            var added = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
            var modified = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(e => e.Entity);

            DbContextTimestamps.AddTimestamps(added, modified);
        }
        #endregion

        #region Save Changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken
            = default(CancellationToken))
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}
