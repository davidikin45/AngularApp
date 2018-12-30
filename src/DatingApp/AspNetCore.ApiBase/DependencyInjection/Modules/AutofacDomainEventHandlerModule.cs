using AspNetCore.ApiBase.DomainEvents;
using Autofac;
using System;
using System.IO;
using System.Linq;

namespace AspNetCore.ApiBase.DependencyInjection.Modules
{
    public class AutofacDomainEventHandlerModule : Module
    {

        public string[] Paths;
        public Func<string, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            foreach (string path in Paths)
            {
                var assemblies = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                              .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
                              .Where(Filter)
                              .Select(System.Reflection.Assembly.LoadFrom);

                foreach (System.Reflection.Assembly assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                              .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                              .Select(p => p);

                    foreach (var type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            if (!type.IsGenericType)
                            {
                                builder.RegisterType(type).As(type);
                                var eventHandlerInterfaces = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                                foreach (var eventHandlerInterface in eventHandlerInterfaces)
                                {
                                    builder.RegisterType(type).As(eventHandlerInterface);
                                }
                            }
                        }
                    }

                   types = assembly.GetTypes()
                              .Where(x => x.GetInterfaces().Any(y => y == typeof(IDynamicDomainEventHandler)))
                              .Select(p => p);

                    foreach (var type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            if (!type.IsGenericType)
                            {
                                builder.RegisterType(type).As(type);
                                var eventHandlerInterfaces = type.GetInterfaces().Where(x => x == typeof(IDynamicDomainEventHandler));
                                foreach (var eventHandlerInterface in eventHandlerInterfaces)
                                {
                                    builder.RegisterType(type).As(eventHandlerInterface);
                                }
                            }
                        }
                    }
                }

            }
        }
    }


}
