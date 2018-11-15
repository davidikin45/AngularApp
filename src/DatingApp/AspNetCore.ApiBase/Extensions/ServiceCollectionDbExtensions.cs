using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.ApiBase.Extensions
{
    public static class ServiceCollectionDbExtensions
    {
        public static IServiceCollection AddDbContextTenant<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            return services.AddDbContext<TContext>();
        }

        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.SetConnectionString(connectionString), contextLifetime);
        }

        public static DbContextOptionsBuilder SetConnectionString(this DbContextOptionsBuilder options, string connectionString, string migrationsAssembly = "")
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }
            if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                if(!string.IsNullOrWhiteSpace(migrationsAssembly))
                {
                    return options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                }
                return options.UseSqlite(connectionString);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                {
                    return options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                }
                return options.UseSqlServer(connectionString);
            }
        }

        //https://medium.com/volosoft/asp-net-core-dependency-injection-best-practices-tips-tricks-c6e9c67f9d96
        public static IServiceCollection AddDbContextInMemory<TContext>(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()), contextLifetime);
        }

        public static IServiceCollection AddDbContextSqlServer<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString), contextLifetime);
        }

        public static IServiceCollection AddDbContextSqlite<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlite(connectionString), contextLifetime);
        }

        public static IServiceCollection AddDbContextPoolSqlServer<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContextPool<TContext>(options =>
                    options.UseSqlServer(connectionString));
        }

        public static IServiceCollection AddDbContextSqlServerWithRetries<TContext>(this IServiceCollection services, string connectionString, int retries = 10, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
           return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: retries,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    }), contextLifetime);
        }

        public static void AddUnitOfWork<TUnitOfWorkImplementation>(this IServiceCollection services)
    where TUnitOfWorkImplementation : UnitOfWorkBase
        {
            services.AddScoped<TUnitOfWorkImplementation>();
        }

        public static void AddUnitOfWork<TUnitOfWork, TUnitOfWorkImplementation>(this IServiceCollection services)
            where TUnitOfWork : class
            where TUnitOfWorkImplementation : UnitOfWorkBase, TUnitOfWork
        {
            services.AddScoped<TUnitOfWork, TUnitOfWorkImplementation>();
        }
    }
}
