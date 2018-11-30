using AspNetCore.ApiBase.Data.Converters;
using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data
{
    public abstract class DbContextBase : DbContext
    {
        protected DbContextBase()
        {
        }

        public DbContextBase(DbContextOptions options)
            : base(options)
        {
        }

        public bool LazyLoadingEnabled
        {
            get { return ChangeTracker.LazyLoadingEnabled; }
            set { ChangeTracker.LazyLoadingEnabled = value; }
        }

        public bool AutoDetectChangesEnabled
        {
            get { return ChangeTracker.AutoDetectChangesEnabled; }
            set { ChangeTracker.AutoDetectChangesEnabled = value; }
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
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            optionsBuilder.UseLoggerFactory(CommandLoggerFactory).EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.RemovePluralizingTableNameConvention();
            builder.AddSoftDeleteFilter();

            builder.AddJsonValues();
            builder.AddLocalizedStringValues();

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

        #region Save Changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.SetTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            this.SetTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.SetTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken
            = default(CancellationToken))
        {
            this.SetTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
}
