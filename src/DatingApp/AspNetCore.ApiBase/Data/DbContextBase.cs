using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data
{
    public abstract class DbContextBase : DbContext
    {
        public DbContextBase(DbContextOptions options)
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
            builder.RemovePluralizingTableNameConvention();
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
