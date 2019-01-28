using Newtonsoft.Json;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Dtos
{
    public class WebApiPagedResponseDto<T> : PagingInfoDto
    {
        [JsonIgnore]
        public List<T> Data
        {
            get { return Rows; }
        }

        public List<T> Rows
        { get; set; }
    }
}
