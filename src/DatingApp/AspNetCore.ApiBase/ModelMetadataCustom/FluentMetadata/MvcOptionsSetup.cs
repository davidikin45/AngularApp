using AspNetCore.ApiBase.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.ApiBase.ModelMetadataCustom.FluentMetadata
{
    public class MvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IMetadataConfiguratorProviderSingleton _provider;

        public MvcOptionsSetup(IMetadataConfiguratorProviderSingleton provider)
        {
            _provider = provider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Insert(0, new FluentMetadataProvider(_provider));
        }
    }
}