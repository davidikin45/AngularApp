using AspNetCore.ApiBase.Dtos;
using GrabMobile.ApiClient.HttpClientREST;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.ApiClient
{
    //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    //HttpClient does not throw an exception when the HTTP response contains an error code.Instead, the IsSuccessStatusCode property is false if the status is an error code.If you prefer to treat HTTP error codes as exceptions, call HttpResponseMessage.EnsureSuccessStatusCode on the response object. EnsureSuccessStatusCode throws an exception if the status code falls outside the range 200–299. Note that HttpClient can throw exceptions for other reasons — for example, if the request times out.
    public class GenericReadOnlyApiClient<TReadDto> : IApiControllerEntityReadOnlyClient<TReadDto>
       where TReadDto : class
    {
        protected readonly HttpClient client;
        protected readonly string resource;
        protected readonly JsonSerializerSettings settings;

        public GenericReadOnlyApiClient(HttpClient client, JsonSerializerSettings settings, string resource)
        {
            this.client = client;
            this.resource = resource;
            this.settings = settings;
        }

        #region Search
        public async Task<WebApiListResponseDto<TReadDto>> SearchAsync(WebApiPagedSearchOrderingRequestDto resourceParameters)
        {
            var response = await client.GetWithQueryString($"{resource}", resourceParameters);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<WebApiListResponseDto<TReadDto>>();
        }
        #endregion

        #region GetAll
        public async Task<List<TReadDto>> GetAllAsync()
        {
            var response = await client.Get($"{resource}/get-all");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<TReadDto>>();
        }

        public async Task<List<TReadDto>> GetAllPagedAsync()
        {
            var response = await client.Get($"{resource}/get-all-paged");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<TReadDto>>();
        }
        #endregion

        #region GetById
        public async Task<TReadDto> GetByIdAsync(string id, WebApiParamsDto parameters = null)
        {
            var response = await client.GetWithQueryString($"{resource}/{id}", parameters);

            TReadDto item = null;
            if (response.IsSuccessStatusCode)
            {
                item = await response.ContentAsTypeAsync<TReadDto>();
            }

            return item;
        }

        public async Task<List<TReadDto>> BulkGetByIdsAsync(IEnumerable<string> ids)
        {
            var response = await client.Get($"{resource}/{String.Join(',', ids)}");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<TReadDto>>();
        }

        public async Task<TReadDto> GetByIdFullGraphAsync(string id, WebApiParamsDto parameters = null)
        {
            var response = await client.GetWithQueryString($"{resource}/full-graph/{id}", parameters);

            TReadDto item = null;
            if (response.IsSuccessStatusCode)
            {
                item = await response.ContentAsTypeAsync<TReadDto>();
            }

            return item;
        }
        #endregion

        #region Child Collection
        public async Task<WebApiListResponseDto<TChildCollectionItemDto>> GetByIdChildCollectionAsync<TChildCollectionItemDto>(string id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters)
     where TChildCollectionItemDto : class
        {
            var response = await client.GetWithQueryString($"{resource}/{id}/{collection}", resourceParameters);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<WebApiListResponseDto<TChildCollectionItemDto>>();
        }

        public async Task<TChildCollectionItemDto> GetByIdChildCollectionItemAsync<TChildCollectionItemDto>(string id, string collection, string collectionItemId)
            where TChildCollectionItemDto : class
        {
            var response = await client.Get($"{resource}/{id}/{collection}/{collectionItemId}");

            TChildCollectionItemDto item = null;
            if (response.IsSuccessStatusCode)
            {
                item = await response.ContentAsTypeAsync<TChildCollectionItemDto>();
            }

            return item;
        }
        #endregion
    }
}