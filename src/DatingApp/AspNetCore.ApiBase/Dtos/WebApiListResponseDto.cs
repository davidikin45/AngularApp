using System.Collections.Generic;

namespace AspNetCore.ApiBase.Dtos
{
    public class WebApiListResponseDto<TDto>
    {
        public IEnumerable<TDto> Value { get; set; }
        public IEnumerable<LinkDto> Links { get; set; }
    }
}
