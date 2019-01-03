using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.ApiBase.HttpClientREST
{
    public static class FormUrlEncodedContentExtensions
    {
        public static FormUrlEncodedContent FromObject(object value)
        {
            return new FormUrlEncodedContent(QueryStringHelper.ToKeyValue(value) ?? new Dictionary<string, string>());
        }
    }
}
