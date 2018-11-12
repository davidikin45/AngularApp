using AspNetCore.ApiBase.ModelMetadataCustom;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.ApiBase.ModelMetadataCustom.Providers
{
    public class AttributeMetadataProvider : IDisplayMetadataProvider
    {
        public AttributeMetadataProvider() { }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.PropertyAttributes != null)
            {
                foreach (object propAttr in context.PropertyAttributes)
                {
                    if(propAttr is IDisplayMetadataAttribute)
                    {
                        ((IDisplayMetadataAttribute)propAttr).TransformMetadata(context);
                    }
                }
            }
        }
    }
}
