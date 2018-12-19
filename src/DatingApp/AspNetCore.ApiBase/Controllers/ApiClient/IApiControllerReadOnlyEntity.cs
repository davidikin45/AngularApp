using AspNetCore.ApiBase.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.ApiClient
{
    public interface IApiControllerEntityReadOnly<TReadDto>
        where TReadDto : class
    {
        Task<ActionResult<WebApiListResponseDto<TReadDto>>> Search(WebApiPagedSearchOrderingRequestDto resourceParameters);

        Task<ActionResult<List<TReadDto>>> GetAll();
        Task<ActionResult<List<TReadDto>>> GetAllPaged();

        Task<ActionResult<TReadDto>> GetById(string id, WebApiParamsDto parameters);
        Task<ActionResult<TReadDto>> GetByIdFullGraph(string id, WebApiParamsDto parameters);

        Task<ActionResult<List<TReadDto>>> BulkGetByIds(IEnumerable<string> ids);

        Task<IActionResult> GetByIdChildCollection(string id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters);
    }

    public interface IApiControllerEntityReadOnlyClient<TReadDto>
         where TReadDto : class
    {
        Task<WebApiListResponseDto<TReadDto>> SearchAsync(WebApiPagedSearchOrderingRequestDto resourceParameters);

        Task<List<TReadDto>> GetAllAsync();
        Task<List<TReadDto>> GetAllPagedAsync();

        Task<TReadDto> GetByIdAsync(object id, WebApiParamsDto parameters);
        Task<TReadDto> GetByIdFullGraphAsync(object id, WebApiParamsDto parameters);

        Task<List<TReadDto>> BulkGetByIdsAsync(IEnumerable<object> ids);

        Task<WebApiListResponseDto<TCollectionItemDto>> GetByIdChildCollectionAsync<TCollectionItemDto>(object id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters) where TCollectionItemDto : class;
        Task<TCollectionItemDto> GetByIdChildCollectionItemAsync<TCollectionItemDto>(object id, string collection, string collectionItemId) where TCollectionItemDto : class;
    }
}
