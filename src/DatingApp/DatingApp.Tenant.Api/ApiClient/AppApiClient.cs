using AspNetCore.ApiBase.ApiClient;
using AspNetCore.ApiBase.Controllers.ApiClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DatingApp.Tenant.Api.ApiClient
{
    public class AppApiClient : ApiClientBase
    {
        public AppApiClient(HttpClient httpClient, ApiClientSettings apiClientSettings, IMemoryCache memoryCache = null, ILogger<AppApiClient> logger = null)
            :base(httpClient, apiClientSettings, memoryCache, logger)
        {

        }

        public override void InitializeRepositories(HttpClient httpClient)
        {
           
        }
    }
}