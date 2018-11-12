using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.ApiBase.ModelMetadataCustom
{
    public interface IDisplayMetadataConventionFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
