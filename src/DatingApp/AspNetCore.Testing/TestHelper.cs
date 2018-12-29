using AspNetCore.ApiBase.Data.Repository;
using AspNetCore.ApiBase.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCore.Testing
{
    public static class TestHelper
    {
        public static void MockCurrentUser(this Controller controller, string userId, string username, string authenticationType)
        {
            controller.MockHttpContext(userId, username, authenticationType);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(string userId, string username, string authenticationType)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, authenticationType));

            return user;
        }

        public static void MockHttpContext(this Controller controller, string userId, string username, string authenticationType)
        {
            var httpContext = FakeAuthenticatedHttpContext(userId, username, authenticationType);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private static HttpContext FakeAuthenticatedHttpContext(string userId, string username, string authenticationType)
        {
            var context = new DefaultHttpContext();
            context.User = CreateClaimsPrincipal(userId, username, authenticationType);
          
            return context;
        }

        public static IConfigurationRoot GetConfiguration(string environmentName = "")
        {
            var environmentNamePart = !string.IsNullOrEmpty(environmentName) ? "." + environmentName : "";
            var fileName = $"appsettings{environmentNamePart}.json";
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(fileName, false)
                .AddEnvironmentVariables()
                .Build();
        }

        public static GenericRepository<TEntity> GetRepository<TContext, TEntity>(string connectionString, bool beginTransaction)
            where TContext : DbContext
            where TEntity : class
        {
            var context = GetContext<TContext>(connectionString, beginTransaction);
            return new GenericRepository<TEntity>(context);
        }

        public static TContext GetContext<TContext>(string connectionString, bool beginTransaction)
          where TContext : DbContext
        {

            DbContextOptions options;
            var builder = new DbContextOptionsBuilder();
            builder.SetConnectionString<TContext>(connectionString);
            options = builder.Options;

            Type type = typeof(TContext);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(DbContextOptions) });
            object instance = ctor.Invoke(new object[] { options });

            TContext context = (TContext)ctor.Invoke(new object[] { options });

            if (beginTransaction)
            {
                context.Database.BeginTransaction();
            }

            return context;
        }

        public static GenericRepository<TEntity> GetRepositoryInMemory<TContext, TEntity>(string databaseName = "")
            where TContext : DbContext
            where TEntity : class
        {
            var context = GetContextInMemory<TContext>(databaseName);
            return new GenericRepository<TEntity>(context);
        }

        public static TContext GetContextInMemory<TContext>(string databaseName = "")
          where TContext : DbContext
        {

            if(string.IsNullOrEmpty(databaseName))
            {
                databaseName = Guid.NewGuid().ToString();
            }

            DbContextOptions options;
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase(databaseName);
            options = builder.Options;

            Type type = typeof(TContext);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(DbContextOptions) });
            object instance = ctor.Invoke(new object[] { options });

            TContext context = (TContext)ctor.Invoke(new object[] { options });
            return context;
        }
    }
}
