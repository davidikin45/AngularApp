using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.ApiBase.ModelMetadataCustom.FluentMetadata
{
    public interface IValidationMetadataConfigurator
    {
        void Configure(ValidationMetadata metadata);
    }
}