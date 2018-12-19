using AspNetCore.ApiBase.Controllers.ApiClient;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace DatingApp.Tenant.Api.ApiClient
{
    public class AppApiClient : ApiClientBase
    {
        public AppApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            :base(httpClient, httpContextAccessor)
        {

        }

        public override void InitializeRepositories(HttpClient httpClient)
        {
           
        }
    }
}