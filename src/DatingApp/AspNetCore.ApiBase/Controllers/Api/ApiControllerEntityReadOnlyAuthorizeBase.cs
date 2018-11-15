using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Data.Helpers;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Dtos;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Helpers;
using AspNetCore.ApiBase.Mapping;
using AspNetCore.ApiBase.ModelBinders;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.Api
{
    //Resource naming - Should be noun = thing, not an action. Example. api/getauthors is bad. api/authors is good.
    //Folow this principle for predictability. Represent hierachy. api/authors/{author}/books
    //Pluralize vs not pluralize is OK. Stay consistent
    //Filters,Sorting,Ordering are not resources. Should not be in URL, use query string instead.


    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize]
    //[AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class ApiControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService> : ApiControllerBase
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {
        public IEntityService Service { get; private set; }
        public ITypeHelperService TypeHelperService { get; private set; }
        public IActionEventsService ActionEventsService { get; private set; }

        public ApiControllerEntityReadOnlyAuthorizeBase(IEntityService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IActionEventsService actionEventsService)
        : base(mapper, emailService, urlHelper, appSettings)
        {
            Service = service;
            TypeHelperService = typeHelperService;
            ActionEventsService = actionEventsService;
        }

        #region List

        /// <summary>
        /// Gets the paged.
        /// </summary>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read, ResourceOperationsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("")]
        [Route(".{format}")]
        [HttpGet]
        [HttpHead]
        [ProducesResponseType(typeof(WebApiPagedResponseDto<object>), 200)]
        public virtual async Task<IActionResult> GetPaged([FromQuery] WebApiPagedSearchOrderingRequestDto resourceParameters)
        {  
            if(User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value.EndsWith(ResourceOperationsCore.CRUD.Operations.Read)).Count() == 0)
            {
                resourceParameters.UserId = UserId;
            }

            return await List(resourceParameters);
        }

        private async Task<IActionResult> List(WebApiPagedSearchOrderingRequestDto resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "Id";

            if (!TypeHelperService.TypeHasProperties<TDto>(resourceParameters.Fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.SearchAsync(cts.Token, resourceParameters.UserId, resourceParameters.Search, null, AutoMapperHelper.GetOrderBy<TDto>(resourceParameters.OrderBy, resourceParameters.OrderType), resourceParameters.Page.HasValue ? resourceParameters.Page - 1 : null, resourceParameters.PageSize, true);

            var totalTask = Service.GetSearchCountAsync(cts.Token, resourceParameters.UserId, resourceParameters.Search, null);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var paginationMetadata = new WebApiPagedResponseDto<TDto>
            {
                Page = resourceParameters.Page.HasValue ? resourceParameters.Page.Value : 1,
                PageSize = resourceParameters.PageSize.HasValue ? resourceParameters.PageSize.Value : data.Count(),
                Records = total,
                PreviousPageLink = null,
                NextPageLink = null
            };

            if (paginationMetadata.HasPrevious)
            {
                paginationMetadata.PreviousPageLink = CreateResourceUri(resourceParameters, ResourceUriType.PreviousPage);
            }

            if (paginationMetadata.HasNext)
            {
                paginationMetadata.NextPageLink = CreateResourceUri(resourceParameters, ResourceUriType.NextPage);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollections(resourceParameters,
              paginationMetadata.HasNext, paginationMetadata.HasPrevious);

            var shapedData = IEnumerableExtensions.ShapeData(data.ToList(), resourceParameters.Fields);

            var shapedDataWithLinks = shapedData.Select(dto =>
            {
                var dtoAsDictionary = dto as IDictionary<string, object>;
                var dtoLinks = CreateLinks(
                    dtoAsDictionary["Id"].ToString(), resourceParameters.Fields);

                dtoAsDictionary.Add("links", dtoLinks);

                return dtoAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDataWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read)]
        [FormatFilter]
        [Route("get-all")]
        [Route("get-all.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetAll()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetAllAsync(cts.Token, null, null, null, true);

            var list = response.ToList();

            return Success(list);
        }

        /// <summary>
        /// Gets all paged.
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read)]
        [FormatFilter]
        [Route("get-all-paged")]
        [Route("get-all-paged.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(WebApiPagedResponseDto<object>), 200)]
        public virtual async Task<IActionResult> GetAllPaged()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token, null, null, null, true);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var paginationMetadata = new WebApiPagedResponseDto<TDto>
            {
                Page = 1,
                PageSize = total,
                Records = total,
                PreviousPageLink = "",
                NextPageLink = ""
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Success(data.ToList());
        }
        #endregion

        #region Details with Composition Properties
        //http://jakeydocs.readthedocs.io/en/latest/mvc/models/formatting.html
        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read, ResourceOperationsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("{id}"), Route("{id}.{format}")]
        //[Route("get/{id}"), Route("get/{id}.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public virtual async Task<IActionResult> Get(string id, [FromQuery] string fields)
        {
            if (!TypeHelperService.TypeHasProperties<TDto>(fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we include the Composition properties which should be any child or join entities.
            var response = await Service.GetByIdAsync(id, cts.Token, true, false);

            if (response == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinks(id, fields);

            var linkedResourceToReturn = response.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

            // Success(shapedData);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read, ResourceOperationsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("({ids})"), Route("({ids}).{format}")]
        //[Route("get/({ids})"), Route("get/({ids}).{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdsAsync(cts.Token, ids, true, false);

            var list = response.ToList();

            if (ids.Count() != list.Count())
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            return Success(list);
        }
        #endregion

        #region Details Full Graph
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read, ResourceOperationsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("full-graph/{id}"), Route("full-graph/{id}.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public virtual async Task<IActionResult> GetFullGraph(string id, [FromQuery] string fields)
        {
            if (!TypeHelperService.TypeHasProperties<TDto>(fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we should get the full graph of Composition and Aggregation Properties
            var response = await Service.GetByIdAsync(id, cts.Token, false, true);

            if (response == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinks(id, fields, true);

            var linkedResourceToReturn = response.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

            // Success(shapedData);
        }
        #endregion

        #region Collection List and Details
        /// <summary>
        /// Gets the paged.
        /// </summary>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceOperationsCore.CRUD.Operations.Read, ResourceOperationsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("{id}/{*collection}")]
        //[Route("{id}/{*collection}.{format}")]
        [HttpGet]
        [HttpHead]
        [ProducesResponseType(typeof(WebApiPagedResponseDto<object>), 200)]
        public virtual async Task<IActionResult> GetCollectionProperty(string id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "Id";

            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return ApiErrorMessage(Messages.CollectionInvalid);
            }

            if (RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return await GetCollectionItem(id, collection, resourceParameters.Fields);
            }

            var collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
            if (!TypeHelperService.TypeHasProperties(collectionItemType, resourceParameters.Fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, resourceParameters.Search, resourceParameters.OrderBy, resourceParameters.OrderType == "asc" ? true : false, resourceParameters.Page.HasValue ? resourceParameters.Page - 1 : null, resourceParameters.PageSize);

            var totalTask = Service.GetByIdWithPagedCollectionPropertyCountAsync(cts.Token, id, collection, resourceParameters.Search);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var result = dataTask.Result;

            var total = totalTask.Result;

            IEnumerable<Object> list = ((IEnumerable<Object>)RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), result));

            var paginationMetadata = new WebApiPagedResponseDto<TDto>
            {
                Page = resourceParameters.Page.HasValue ? resourceParameters.Page.Value : 1,
                PageSize = resourceParameters.PageSize.HasValue ? resourceParameters.PageSize.Value : list.Count(),
                Records = total,
                PreviousPageLink = null,
                NextPageLink = null
            };

            if (paginationMetadata.HasPrevious)
            {
                paginationMetadata.PreviousPageLink = CreateCollectionPropertyResourceUri(collection, resourceParameters, ResourceUriType.PreviousPage);
            }

            if (paginationMetadata.HasNext)
            {
                paginationMetadata.NextPageLink = CreateCollectionPropertyResourceUri(collection, resourceParameters, ResourceUriType.NextPage);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollectionProperty(collection, resourceParameters, paginationMetadata.HasNext, paginationMetadata.HasPrevious);

            var shapedData = IEnumerableExtensions.ShapeData(list, collectionItemType, resourceParameters.Fields);

            var shapedDataWithLinks = shapedData.Select(collectionPropertyDtoItem =>
            {
                var collectionPropertyDtoItemAsDictionary = collectionPropertyDtoItem as IDictionary<string, object>;
                var collectionPropertyDtoItemLinks = CreateLinksForCollectionItem(id, collection + "/" + collectionPropertyDtoItemAsDictionary["Id"].ToString(), resourceParameters.Fields);

                collectionPropertyDtoItemAsDictionary.Add("links", collectionPropertyDtoItem);

                return collectionPropertyDtoItemAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDataWithLinks
                ,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        private async Task<IActionResult> GetCollectionItem(string id, string collection, [FromQuery] string fields)
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return ApiErrorMessage(Messages.CollectionInvalid);
            }

            if (!RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return ApiErrorMessage(Messages.CollectionInvalid);
            }

            var collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
            if (!TypeHelperService.TypeHasProperties(collectionItemType, fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, "", null, false, null, null);

            var collectionItem = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), response);

            if (collectionItem == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinksForCollectionItem(id, collection, fields);

            var linkedResourceToReturn = collectionItem.ShapeData(collectionItemType, fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }
        #endregion

        #region HttpOptions
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, PATCH");
            return Ok();
        }
        #endregion

        #region HATEOAS
        private string CreateResourceUri(
WebApiPagedSearchOrderingRequestDto resourceParameters,
ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return UrlHelper.Action(nameof(GetPaged),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          userId = resourceParameters.UserId,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page - 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
                case ResourceUriType.NextPage:
                    return UrlHelper.Action(nameof(GetPaged),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          userId = resourceParameters.UserId,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page + 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);

                default:
                    return UrlHelper.Action(nameof(GetPaged),
                    UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                    new
                    {
                        fields = resourceParameters.Fields,
                        userId = resourceParameters.UserId,
                        orderBy = resourceParameters.OrderBy,
                        search = resourceParameters.Search,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
            }
        }

        private string CreateCollectionPropertyResourceUri(
            string collection,
            WebApiPagedRequestDto resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return UrlHelper.Action(nameof(GetCollection),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          collection = collection,
                          fields = resourceParameters.Fields,
                          page = resourceParameters.Page - 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
                case ResourceUriType.NextPage:
                    return UrlHelper.Action(nameof(GetCollection),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          collection = collection,
                          fields = resourceParameters.Fields,
                          page = resourceParameters.Page + 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);

                default:
                    return UrlHelper.Action(nameof(GetCollection),
                    UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                    new
                    {
                        collection = collection,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
            }
        }

        protected IEnumerable<LinkDto> CreateLinksForCreate()
        {
            var links = new List<LinkDto>();

            links.Add(
           new LinkDto(UrlHelper.Action("Create", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), UrlHelper.ActionContext.HttpContext.Request.Scheme),
           "create",
           HttpMethod.Post.Method));

            return links;
        }

        protected IEnumerable<LinkDto> CreateLinks(string id, string fields, bool fullGraph = false)
        {
            var links = new List<LinkDto>();

            string action = nameof(Get);
            if(fullGraph)
            {
                action = nameof(GetFullGraph);
            }

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(action, UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }
            else
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(action, UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id, fields = fields }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }

            links.Add(
              new LinkDto(UrlHelper.Action("Delete", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
              "delete",
              HttpMethod.Delete.Method));

            links.Add(
                new LinkDto(UrlHelper.Action("Update", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                "update",
                 HttpMethod.Put.Method));

            links.Add(
                new LinkDto(UrlHelper.Action("UpdatePartial", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                "partially_update",
                new HttpMethod("PATCH").Method));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollectionItem(string id, string collection, string fields)
        {
            var links = new List<LinkDto>();

            //Create links for Collection Item Get, Delete and Update. Not sure if we want to allow 

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(nameof(GetCollection), UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new {  id = id, collection = collection }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }
            else
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(nameof(GetCollection), UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id, collection = collection, fields = fields }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }

            //Create links for Collection Item Delete and Update. Not sure if we want to allow this.

            //links.Add(
            //  new LinkDto(UrlHelper.Action("Delete", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
            //  "delete",
            //  "DELETE"));

            //links.Add(
            //    new LinkDto(UrlHelper.Action("Update", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
            //    new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
            //    "update",
            //    "PUT"));

            //links.Add(
            //    new LinkDto(UrlHelper.Action("UpdatePartial", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
            //    new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
            //    "partially_update",
            //    "PATCH"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollections(WebApiPagedSearchOrderingRequestDto resourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateResourceUri(resourceParameters,
               ResourceUriType.Current)
               , "self", HttpMethod.Get.Method));

            links.Add(
           new LinkDto(UrlHelper.Action("Create", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
          null, UrlHelper.ActionContext.HttpContext.Request.Scheme),
           "add",
           HttpMethod.Post.Method));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateResourceUri(resourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", HttpMethod.Get.Method));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", HttpMethod.Get.Method));
            }

            return links;
        }

        /// <summary>
        /// Creates the links for collection property.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <param name="hasNext">if set to <c>true</c> [has next].</param>
        /// <param name="hasPrevious">if set to <c>true</c> [has previous].</param>
        /// <returns></returns>
        private IEnumerable<LinkDto> CreateLinksForCollectionProperty(string collection, WebApiPagedRequestDto resourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
               ResourceUriType.Current)
               , "self", HttpMethod.Get.Method));

            //Todo if want to allow Add to collection property
          //  links.Add(
          // new LinkDto(UrlHelper.Action("Create", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
          //null, UrlHelper.ActionContext.HttpContext.Request.Scheme),
          // "add",
          // "POST"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", HttpMethod.Get.Method));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", HttpMethod.Get.Method));
            }

            return links;
        }
        #endregion
    }
}

