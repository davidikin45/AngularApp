using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase.MultiTenancy.DependencyInjection
{
    public static class AutofacMultiTenancyExtensions
    {
        public static IServiceCollection AddAutofacMultitenant<TContext, TTenant>(this IServiceCollection services, Action<MultitenantContainer> mtcSetter, Assembly assembly)
          where TContext : DbContextTenantsBase<TTenant>
          where TTenant : AppTenant
        {
            var types = assembly
              .GetExportedTypes()
              .Where(type => typeof(ITenantConfiguration).IsAssignableFrom(type))
              .Where(type => (type.IsAbstract == false) && (type.IsInterface == false));

            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new AutofacMultiTenantServiceProviderFactory<TContext, TTenant>(mtcSetter, types));
        }

        public static IWebHostBuilder UseAutofacMultiTenant<TContext, TTenant>(this IWebHostBuilder builder, Assembly assembly)
              where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            MultitenantContainer multiTenantContainer = null;
            Func<MultitenantContainer> multitenantContainerAccessor = () => multiTenantContainer;
            Action<MultitenantContainer> multitenantContainerSetter = (mtc) => { multiTenantContainer = mtc; };
            builder.ConfigureServices(services => services.AddAutofacMultitenant<TContext, TTenant>(multitenantContainerSetter, assembly));
            builder.ConfigureServices(services => services.AddSingleton((sp) => multiTenantContainer));
            return builder.UseAutofacMultitenantRequestServices(multitenantContainerAccessor);

        }

        private class AutofacMultiTenantServiceProviderFactory<TContext, TTenant> : IServiceProviderFactory<ContainerBuilder>
              where TContext : DbContextTenantsBase<TTenant>
              where TTenant : AppTenant
        {
            private Action<MultitenantContainer> _mtcSetter;
            private readonly IEnumerable<Type> _types;

            public AutofacMultiTenantServiceProviderFactory(Action<MultitenantContainer> mtcSetter, IEnumerable<Type> types)
            {
                _mtcSetter = mtcSetter;
                _types = types;
            }

            public ContainerBuilder CreateBuilder(IServiceCollection services)
            {
                var containerBuilder = new ContainerBuilder();

                containerBuilder.Populate(services);

                return containerBuilder;
            }

            public IServiceProvider CreateServiceProvider(ContainerBuilder builder)
            {
                var container = builder.Build();

                var tenantIdentificationStrategy = container.Resolve<ITenantIdentificationService<TContext, TTenant>>();
                var mtc = new MultitenantContainer(tenantIdentificationStrategy, container);

                var configuration = container.Resolve<IConfiguration>();
                var environment = container.Resolve<IHostingEnvironment>();

                _mtcSetter(mtc);

                AddTenantServices(mtc, configuration, environment);

                return new AutofacServiceProvider(mtc);
            }

            private void AddTenantServices(MultitenantContainer mtc, IConfiguration configuration, IHostingEnvironment environment)
            {
                var instances = _types
                    .Select(type => Activator.CreateInstance(type))
                    .OfType<ITenantConfiguration>().ToList();

                foreach (var instance in instances)
                {
                    var actionBuilder = new ConfigurationActionBuilder();
                    instance.ConfigureServices(actionBuilder, configuration, environment);
                    mtc.ConfigureTenant(instance.TenantId, actionBuilder.Build());
                }
            }
        }
    }
}
