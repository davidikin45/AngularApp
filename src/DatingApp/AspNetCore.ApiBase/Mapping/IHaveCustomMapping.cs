using AutoMapper;

namespace AspNetCore.ApiBase.Mapping
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
