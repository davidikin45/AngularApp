using AspNetCore.ApiBase.Alerts;
using AspNetCore.ApiBase.Dtos;
using GrabMobile.ApiClient.HttpClientREST;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.ApiClient
{
    public class GenericApiClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : GenericReadOnlyApiClient<TReadDto>, IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
    {
        public GenericApiClient(HttpClient client, JsonSerializerSettings settings, string resource)
            :base(client, settings, resource)
        {

        }

        #region New Instance
        public async Task<TCreateDto> NewDefaultAsync()
        {
            var response = await client.Get($"{resource}/new");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<TCreateDto>();
        }
        #endregion

        #region Create
        public async Task<TReadDto> CreateAsync(TCreateDto dto)
        {
            var response = await client.Post($"{resource}", dto, settings);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<TReadDto>();
        }
        #endregion

        #region Bulk Create
        public async Task<List<WebApiMessage>> BulkCreateAsync(TCreateDto[] dtos)
        {
            var response = await client.Post($"{resource}/bulk", dtos, settings);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<WebApiMessage>>();
        }
        #endregion

        #region Get for Edit
        public async Task<TUpdateDto> GetByIdForEditAsync(string id)
        {
            var response = await client.Get($"{resource}/edit/{id}");

            TUpdateDto item = null;
            if (response.IsSuccessStatusCode)
            {
                item = await response.ContentAsTypeAsync<TUpdateDto>();
            }

            return item;
        }
        #endregion

        #region Bulk Get for Edit
        public async Task<List<TUpdateDto>> BulkGetByIdsForEditAsync(IEnumerable<string> ids)
        {
            var response = await client.Get($"{resource}/bulk/edit/{String.Join(',', ids)}");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<TUpdateDto>>();
        }
        #endregion

        #region Update
        public async Task UpdateAsync(string id, TUpdateDto dto)
        {
            var response = await client.Put($"{resource}/{id}", dto, settings);
            await response.EnsureSuccessStatusCodeAsync();
        }
        #endregion

        #region Bulk Update
        public async Task<List<WebApiMessage>> BulkUpdateAsync(BulkDto<TUpdateDto>[] dtos)
        {
            var response = await client.Put($"{resource}/bulk", dtos, settings);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<WebApiMessage>>();
        }
        #endregion

        #region Partial Update
        public async Task UpdatePartialAsync(string id, JsonPatchDocument dtoPatch)
        {
            var response = await client.Patch($"{resource}/{id}", dtoPatch, settings);

            await response.EnsureSuccessStatusCodeAsync();
        }
        #endregion

        #region Bulk Partial Update
        public async Task<List<WebApiMessage>> BulkUpdatePartialAsync(BulkDto<JsonPatchDocument>[] dtos)
        {
            var response = await client.Patch($"{resource}/bulk", dtos, settings);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<WebApiMessage>>();
        }
        #endregion

        #region Get For Delete
        public async Task<TDeleteDto> GetByIdForDeleteAsync(string id)
        {
            var response = await client.Get($"{resource}/delete/{id}");

            TDeleteDto item = null;
            if (response.IsSuccessStatusCode)
            {
                item = await response.ContentAsTypeAsync<TDeleteDto>();
            }

            return item;
        }
        #endregion

        #region Bulk Get For Delete
        public async Task<List<TDeleteDto>> BulkGetByIdsForDeleteAsync(IEnumerable<string> ids)
        {
            var response = await client.Get($"{resource}/bulk/delete/{String.Join(',', ids)}");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<TDeleteDto>>();
        }
        #endregion

        #region Delete
        public async Task DeleteAsync(string id, [FromBody] TDeleteDto dto)
        {
            var response = await client.Delete($"{resource}/{id}", dto, settings);

            await response.EnsureSuccessStatusCodeAsync();
        }
        #endregion

        #region Bulk Delete
        public async Task<List<WebApiMessage>> BulkDeleteAsync([FromBody] TDeleteDto[] dtos)
        {
            var response = await client.Delete($"{resource}/bulk", dtos, settings);

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<List<WebApiMessage>>();
        }
        #endregion

        #region Child Collection Item
        public async Task<CollectionItemTypeDto> NewCollectionItemAsync<CollectionItemTypeDto>(string collection)
        {
            var response = await client.Get($"{resource}/new/{collection}");

            await response.EnsureSuccessStatusCodeAsync();

            return await response.ContentAsTypeAsync<CollectionItemTypeDto>();
        }
        #endregion
    }
}
