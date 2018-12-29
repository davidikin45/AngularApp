using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase
{
    public class Config
    {
        private static readonly Dictionary<string, string> InMemoryDefaults = new Dictionary<string, string> { {
                WebHostDefaults.EnvironmentKey, "Development"
            } };

        public static IConfigurationRoot Build(string[] args, string contentRoot)
        {
            var configEnvironmentBuilder = new ConfigurationBuilder()
                   .AddInMemoryCollection(InMemoryDefaults)
                   .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
            {
                configEnvironmentBuilder.AddCommandLine(args);
            }
            var configEnvironment = configEnvironmentBuilder.Build();

            var appSettingsFileName = "appsettings.json";
            var appSettingsEnvironmentFilename = "appsettings." + (configEnvironment[WebHostDefaults.EnvironmentKey] ?? "Production") + ".json";

            Console.WriteLine($"Loading Settings:" + Environment.NewLine +
                               $"{contentRoot}\\{appSettingsFileName}" + Environment.NewLine +
                               $"{contentRoot}\\{appSettingsEnvironmentFilename}");

            var config = new ConfigurationBuilder()
           .AddInMemoryCollection(InMemoryDefaults)
           .SetBasePath(contentRoot)
           .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
           .AddJsonFile(appSettingsEnvironmentFilename, optional: true, reloadOnChange: true)
           .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
            {
                config.AddCommandLine(args);
            }

            return config.Build();
        }
    }
}