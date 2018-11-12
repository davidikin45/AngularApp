using Autofac;
using System;
using System.IO;
using System.Linq;

namespace LeacockSite.DependencyInjection.Autofac.Modules
{
    public class AutofacConventionsDependencyInjectionModule : Module
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
                              .Where(p => p.GetInterface("I" + p.Name) != null)
                              .Select(p => p);

                    //  #4
                    foreach (var type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            if (type.IsGenericType)
                            {
                                builder.RegisterGeneric(type).As(type.GetInterface("I" + type.Name));
                            }
                            else
                            {
                                if (type.Name.Contains("Singleton"))
                                {
                                    builder.RegisterType(type).As(type.GetInterface("I" + type.Name)).SingleInstance();
                                }
                                else
                                {
                                    builder.RegisterType(type).As(type.GetInterface("I" + type.Name));
                                }
                            }
                        }
                    }
                }
            }
        }
    }


}
