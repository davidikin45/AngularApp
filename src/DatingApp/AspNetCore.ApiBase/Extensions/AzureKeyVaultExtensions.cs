using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace AspNetCore.ApiBase.Extensions
{
    public static class AzureKeyVaultExtensions
    {
        public static IWebHostBuilder UseAzureKeyVault(IWebHostBuilder webHostBuilder, string vaultName)
        {
            return webHostBuilder.ConfigureAppConfiguration((ctx, builder) =>
            {
                var config = builder.Build();

                if (!ctx.HostingEnvironment.IsDevelopment())
                {
                    //Section--Name
                    var vaultUrl = config["KeyVault:BaseUrl"];
                    var tokenProvider = new AzureServiceTokenProvider();
                    var kvClient = new KeyVaultClient((authority, resource, scope) => tokenProvider.KeyVaultTokenCallback(authority, resource, scope));
                    builder.AddAzureKeyVault($"https://{vaultName}.vault.azure.net", kvClient, new DefaultKeyVaultSecretManager());
                }
            });
        }
    }
}
