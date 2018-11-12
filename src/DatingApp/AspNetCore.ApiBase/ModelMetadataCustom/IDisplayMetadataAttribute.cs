using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.ApiBase.ModelMetadataCustom
{
    public interface IDisplayMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
