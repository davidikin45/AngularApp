using AspNetCore.ApiBase.Dtos;
using AspNetCore.ApiBase.HttpClientREST;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace AspNetCore.ApiBase.Tests
{
    public class ApiTests
    {
        [Fact]
        public async void GetWithQueryString()
        {
            var fields = "abc";

            var url = QueryHelpers.AddQueryString("http://www.google.com", QueryStringHelper.ToKeyValue(fields));
        }
    }
}
