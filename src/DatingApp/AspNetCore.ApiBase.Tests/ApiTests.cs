using AspNetCore.ApiBase.Dtos;
using AspNetCore.ApiBase.HttpClientREST;
using AspNetCore.ApiBase.Localization;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace AspNetCore.ApiBase.Tests
{
    public class ApiTests
    {
        [Fact]
        public async void GetWithQueryString()
        {

            var p = new WebApiPagedSearchOrderingRequestDto();
            p.Fields = "11";

            var url = QueryHelpers.AddQueryString("http://www.google.com", QueryStringHelper.ToKeyValue(p));
        }
    }
}
