using AspNetCore.ApiBase.ModelMetadataCustom;
using AspNetCore.ApiBase.ModelMetadataCustom.FluentMetadata;
using AspNetCore.ApiBase.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.ApiBase
{
    //https://andrewlock.net/accessing-services-when-configuring-mvcoptions-in-asp-net-core/
    public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IDisplayMetadataConventionFilter[] _metadataFilters;
        private readonly IMetadataConfiguratorProviderSingleton _provider;

        public ConfigureMvcOptions(IDisplayMetadataConventionFilter[] metadataFilters, IMetadataConfiguratorProviderSingleton provider)
        {
            _metadataFilters = metadataFilters;
            _provider = provider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new FluentMetadataProvider(_provider));
            options.ModelMetadataDetailsProviders.Add(new AttributeMetadataProvider());
            options.ModelMetadataDetailsProviders.Add(new ConventionsMetadataProvider(_metadataFilters));
        }
    }
}
