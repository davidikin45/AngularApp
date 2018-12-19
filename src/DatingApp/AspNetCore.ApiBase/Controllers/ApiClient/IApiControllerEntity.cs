using AspNetCore.ApiBase.Alerts;
using AspNetCore.ApiBase.Dtos;
using AspNetCore.ApiBase.Validation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.ApiClient
{
    public interface IApiControllerEntity<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IApiControllerEntityReadOnly<TReadDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
    {
        ActionResult<TCreateDto> NewDefault();

        Task<ActionResult<TReadDto>> Create(TCreateDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkCreate(TCreateDto[] dtos);

        Task<ActionResult<TUpdateDto>> GetByIdForEdit(string id);
        Task<ActionResult<List<TUpdateDto>>> BulkGetByIdsForEditAsync(IEnumerable<string> ids);

        Task<IActionResult> Update(string id, TUpdateDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkUpdate(BulkDto<TUpdateDto>[] dtos);

        Task<IActionResult> UpdatePartial(string id, JsonPatchDocument dtoPatch);
        Task<ActionResult<List<WebApiMessage>>> BulkUpdatePartial(BulkDto<JsonPatchDocument>[] dtos);

        Task<ActionResult<TDeleteDto>> GetByIdForDelete(string id);
        Task<ActionResult<List<TDeleteDto>>> BulkGetByIdsForDeleteAsync(IEnumerable<string> ids);

        Task<IActionResult> DeleteDto(string id, [FromBody] TDeleteDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkDelete([FromBody] TDeleteDto[] dtos);

        IActionResult NewCollectionItem(string collection);
    }

    public interface IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IApiControllerEntityReadOnlyClient<TReadDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
    {
        Task<TCreateDto> NewDefaultAsync();

        Task<TReadDto> CreateAsync(TCreateDto dto);
        Task<List<WebApiMessage>> BulkCreateAsync(TCreateDto[] dtos);

        Task<TUpdateDto> GetByIdForEditAsync(string id);
        Task<List<TUpdateDto>> BulkGetByIdsForEditAsync(IEnumerable<string> ids);

        Task UpdateAsync(string id, TUpdateDto dto);
        Task<List<WebApiMessage>> BulkUpdateAsync(BulkDto<TUpdateDto>[] dtos);

        Task UpdatePartialAsync(string id, JsonPatchDocument dtoPatch);
        Task<List<WebApiMessage>> BulkUpdatePartialAsync(BulkDto<JsonPatchDocument>[] dtos);

        Task<TDeleteDto> GetByIdForDeleteAsync(string id);
        Task<List<TDeleteDto>> BulkGetByIdsForDeleteAsync(IEnumerable<string> ids);

        Task DeleteAsync(string id, [FromBody] TDeleteDto dto);
        Task<List<WebApiMessage>> BulkDeleteAsync([FromBody] TDeleteDto[] dtos);

        Task<CollectionItemTypeDto> NewCollectionItemAsync<CollectionItemTypeDto>(string collection);
    }
}
