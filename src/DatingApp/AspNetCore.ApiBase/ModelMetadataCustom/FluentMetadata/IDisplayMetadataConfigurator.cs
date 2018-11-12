using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.ApiBase.ModelMetadataCustom.FluentMetadata
{
    public interface IDisplayMetadataConfigurator
    {
        void Configure(DisplayMetadata metadata);
    }
}