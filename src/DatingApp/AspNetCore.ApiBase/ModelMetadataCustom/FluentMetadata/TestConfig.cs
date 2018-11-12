namespace AspNetCore.ApiBase.ModelMetadataCustom.FluentMetadata
{
    public class DynamicConfig : ModelMetadataConfiguration<dynamic>
    {
        public DynamicConfig()
        { 
            //Configure<string>("TenantName").Required();
        }
    }
}
