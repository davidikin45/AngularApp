using AspNetCore.ApiBase.Controllers.ApiClient;
using System.Net.Http;

namespace DatingApp.Tenant.Api.ApiClient
{
    public class AppApiClient : ApiClientBase
    {
        public AppApiClient(HttpClient httpClient)
            :base(httpClient)
        {

        }

        public override void InitializeRepositories(HttpClient httpClient)
        {
           
        }
    }
}