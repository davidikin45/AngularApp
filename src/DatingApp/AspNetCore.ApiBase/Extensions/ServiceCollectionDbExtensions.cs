using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.ApiBase.Extensions
{
    public static class ServiceCollectionDbExtensions
    {
        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.SetConnectionString(connectionString), ServiceLifetime.Scoped);
        }

        public static DbContextOptionsBuilder SetConnectionString(this DbContextOptionsBuilder options, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }
            if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                return options.UseSqlite(connectionString);
            }
            else
            {
                return options.UseSqlServer(connectionString);
            }
        }

        //https://medium.com/volosoft/asp-net-core-dependency-injection-best-practices-tips-tricks-c6e9c67f9d96
        public static IServiceCollection AddDbContextInMemory<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddDbContextSqlServer<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddDbContextSqlite<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlite(connectionString), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddDbContextPoolSqlServer<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            return services.AddDbContextPool<TContext>(options =>
                    options.UseSqlServer(connectionString));
        }

        public static IServiceCollection AddDbContextSqlServerWithRetries<TContext>(this IServiceCollection services, string connectionString, int retries = 10) where TContext : DbContext
        {
           return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: retries,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    }), ServiceLifetime.Scoped);
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
