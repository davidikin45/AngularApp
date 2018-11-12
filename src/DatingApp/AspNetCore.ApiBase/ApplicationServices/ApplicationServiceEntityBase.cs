using AspNetCore.ApiBase.Data.Helpers;
using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.Domain;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Dtos;
using AspNetCore.ApiBase.SignalR;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.ApplicationServices
{
    public abstract class ApplicationServiceEntityBase<TEntity, TCreateDto, TReadDto, TUpdateDto, TDeleteDto, TUnitOfWork> : ApplicationServiceEntityReadOnlyBase<TEntity, TReadDto, TUnitOfWork>, IApplicationServiceEntity<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
          where TEntity : class
          where TCreateDto : class
          where TReadDto : class
          where TUpdateDto : class
          where TDeleteDto : class
          where TUnitOfWork : IUnitOfWork
    {

        private readonly IHubContext<ApiNotificationHub<TReadDto>> HubContext;

        public ApplicationServiceEntityBase(string serviceName, TUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService, IHubContext<ApiNotificationHub<TReadDto>> hubContext)
          : this(serviceName, unitOfWork, mapper, authorizationService, userService, validationService, actionEventsService)
        {
            HubContext = hubContext;
        }

        public ApplicationServiceEntityBase(string serviceName, TUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService)
           : base(serviceName, unitOfWork, mapper, authorizationService, userService, validationService, actionEventsService)
        {

        }

        #region GetCreateDefaultDto
        public virtual TCreateDto GetCreateDefaultDto()
        {
            var bo = (TEntity)Activator.CreateInstance(typeof(TEntity));
            return Mapper.Map<TCreateDto>(bo);
        }
        #endregion

        #region Create

        public virtual Result<TReadDto> Create(TCreateDto dto, string createdBy)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            Repository.Add(bo, createdBy);

            var result = UnitOfWork.Save();

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail<TReadDto>(result.ObjectValidationErrors);
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(bo);

            if (HubContext != null)
            {
                HubContext.CreatedAsync(readDto).Wait();
            }

            return Result.Ok(readDto);
        }

        public virtual async Task<Result<TReadDto>> CreateAsync(TCreateDto dto, string createdBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            Repository.Add(bo, createdBy);

            var result = await UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail<TReadDto>(result.ObjectValidationErrors);
                    case ErrorType.DatabaseValidationFailed:
                        return Result.ObjectValidationFail<TReadDto>(result.ObjectValidationErrors);
                    case ErrorType.ObjectDoesNotExist:
                        return Result.ObjectDoesNotExist<TReadDto>();
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(bo);

            if (HubContext != null)
            {
                await HubContext.CreatedAsync(readDto);
            }

            return Result.Ok(readDto);
        }
        #endregion

        #region Bulk Create
        public virtual List<Result> BulkCreate(TCreateDto[] dtos, string createdBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Create(dto, createdBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkCreateAsync(TCreateDto[] dtos, string createdBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                var result = await CreateAsync(dto, createdBy, cancellationToken);
                results.Add(result);
            }
            return results;
        }
        #endregion

        #region GetUpdateDtoById
        public virtual TUpdateDto GetUpdateDtoById(object id)
        {
            var bo = Repository.GetById(id, true);
            return Mapper.Map<TUpdateDto>(bo);
        }

        public virtual async Task<TUpdateDto> GetUpdateDtoByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await Repository.GetByIdAsync(cancellationToken, id, true);
            return Mapper.Map<TUpdateDto>(bo);
        }

        #endregion

        #region GetUpdateDtosByIds
        public virtual IEnumerable<TUpdateDto> GetUpdateDtosByIds(IEnumerable<object> ids)
        {
            var result = Repository.GetByIds(ids);
            return Mapper.Map<IEnumerable<TUpdateDto>>(result);
        }

        public virtual async Task<IEnumerable<TUpdateDto>> GetUpdateDtosByIdsAsync(CancellationToken cancellationToken,
       IEnumerable<object> ids)
        {
            var result = await Repository.GetByIdsAsync(cancellationToken, ids).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TUpdateDto>>(result);
        }
        #endregion

        #region Update

        public virtual Result Update(object id, TUpdateDto dto, string updatedBy)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var persistedBO = Repository.GetById(id);

            Mapper.Map(dto, persistedBO);

            Repository.Update(persistedBO, updatedBy);

            var result = UnitOfWork.Save();

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(persistedBO);

            if (HubContext != null)
            {
                HubContext.UpdatedAsync(readDto).Wait();
            }

            return Result.Ok();
        }

        public virtual Result UpdateGraph(object id, TUpdateDto dto, string updatedBy)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var persistedBO = Repository.GetById(id, true);

            Mapper.Map(dto, persistedBO);

            Repository.Update(persistedBO, updatedBy);

            var result = UnitOfWork.Save();

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(persistedBO);

            if (HubContext != null)
            {
                HubContext.UpdatedAsync(readDto).Wait();
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var persistedBO = await Repository.GetByIdAsync(cancellationToken, id);

            Mapper.Map(dto, persistedBO);

            Repository.Update(persistedBO, updatedBy);

            var result = await UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(persistedBO);

            if (HubContext != null)
            {
                await HubContext.UpdatedAsync(readDto);
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateGraphAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = ValidationService.ValidateObject(dto);
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var persistedBO = await Repository.GetByIdAsync(cancellationToken, id, true);

            Mapper.Map(dto, persistedBO);

            Repository.Update(persistedBO, updatedBy);

            var result = await UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            var readDto = Mapper.Map<TReadDto>(persistedBO);

            if (HubContext != null)
            {
                await HubContext.UpdatedAsync(readDto);
            }


            return Result.Ok();
        }
        #endregion

        #region Bulk Update
        public virtual List<Result> BulkUpdate(BulkDto<TUpdateDto>[] dtos, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Update(dto.Id, dto.Value, updatedBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }

            }
            return results;
        }

        public virtual List<Result> BulkUpdateGraph(BulkDto<TUpdateDto>[] dtos, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = UpdateGraph(dto.Id, dto.Value, updatedBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkUpdateAsync(BulkDto<TUpdateDto>[] dtos, string updatedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await UpdateAsync(dto.Id, dto.Value, updatedBy, cancellationToken);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkUpdateGraphAsync(BulkDto<TUpdateDto>[] dtos, string updatedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await UpdateGraphAsync(dto.Id, dto.Value, updatedBy, cancellationToken);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region Update Partial

        public virtual Result UpdatePartial(object id, JsonPatchDocument dtoPatch, string updatedBy)
        {
            var dto = GetUpdateDtoById(id);

            if (dto == null)
            {
                return Result.ObjectDoesNotExist();
            }

            var ops = new List<Operation<TUpdateDto>>();
            foreach (var op in dtoPatch.Operations)
            {
                ops.Add(new Operation<TUpdateDto>(op.op, op.path, op.from, op.value));
            }

            var dtoPatchTypes = new JsonPatchDocument<TUpdateDto>(ops, dtoPatch.ContractResolver);

            dtoPatchTypes.ApplyTo(dto);

            var result = UpdateGraph(id, dto, updatedBy);

            return Result.Ok();
        }

        public virtual async Task<Result> UpdatePartialAsync(object id, JsonPatchDocument dtoPatch, string updatedBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dto = await GetUpdateDtoByIdAsync(id, cancellationToken);

            if (dto == null)
            {
                return Result.ObjectDoesNotExist();
            }

            var ops = new List<Operation<TUpdateDto>>();
            foreach (var op in dtoPatch.Operations)
            {
                ops.Add(new Operation<TUpdateDto>(op.op, op.path, op.from, op.value));
            }

            var dtoPatchTypes = new JsonPatchDocument<TUpdateDto>(ops, dtoPatch.ContractResolver);

            dtoPatchTypes.ApplyTo(dto);

            var result = await UpdateGraphAsync(id, dto, updatedBy, cancellationToken);

            return Result.Ok();
        }
        #endregion

        #region Bulk Partial Update
        public virtual List<Result> BulkUpdatePartial(BulkDto<JsonPatchDocument>[] dtoPatches, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtoPatches)
            {
                try
                {
                    var result = UpdatePartial(dto.Id, dto.Value, updatedBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public virtual async Task<List<Result>> BulkUpdatePartialAsync(BulkDto<JsonPatchDocument>[] dtoPatches, string updatedBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var results = new List<Result>();
            foreach (var dto in dtoPatches)
            {
                try
                {
                    var result = await UpdatePartialAsync(dto.Id, dto.Value, updatedBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region GetDeleteDtoById
        public virtual TDeleteDto GetDeleteDtoById(object id)
        {
            var bo = Repository.GetById(id);
            return Mapper.Map<TDeleteDto>(bo);
        }

        public virtual async Task<TDeleteDto> GetDeleteDtoByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await Repository.GetByIdAsync(cancellationToken, id);
            return Mapper.Map<TDeleteDto>(bo);
        }
        #endregion

        #region GetDeleteDtosByIds
        public virtual IEnumerable<TDeleteDto> GetDeleteDtosByIds(IEnumerable<object> ids)
        {
            var result = Repository.GetByIds(ids);
            return Mapper.Map<IEnumerable<TDeleteDto>>(result);
        }

        public virtual async Task<IEnumerable<TDeleteDto>> GetDeleteDtosByIdsAsync(CancellationToken cancellationToken,
       IEnumerable<object> ids)
        {
            var result = await Repository.GetByIdsAsync(cancellationToken, ids).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TDeleteDto>>(result);
        }
        #endregion

        #region Delete

        public virtual Result Delete(object id, string deletedBy)
        {
            TDeleteDto deleteDto = GetDeleteDtoById(id);
            return Delete(deleteDto, deletedBy);
        }

        public virtual async Task<Result> DeleteAsync(object id, string deletedBy, CancellationToken cancellationToken)
        {
            TDeleteDto deleteDto = await GetDeleteDtoByIdAsync(id, cancellationToken);
            return await DeleteAsync(deleteDto, deletedBy, cancellationToken).ConfigureAwait(false);
        }

        public virtual Result Delete(TDeleteDto dto, string deletedBy)
        {
            var bo = Mapper.Map<TEntity>(dto);
            Repository.Delete(bo, deletedBy);

            var result = UnitOfWork.Save();

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            if (HubContext != null)
            {
                HubContext.DeletedAsync(dto).Wait();
            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TDeleteDto dto, string deletedBy, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);

            Repository.Delete(bo, deletedBy);

            var result = await UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            if (HubContext != null)
            {
                await HubContext.DeletedAsync(dto);
            }

            return Result.Ok();
        }

        #endregion

        #region Bulk Delete
        public virtual List<Result> BulkDelete(TDeleteDto[] dtos, string deletedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Delete(dto, deletedBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkDeleteAsync(TDeleteDto[] dtos, string deletedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await DeleteAsync(dto, deletedBy, cancellationToken);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region GetCreateDefaultCollectionItemDto
        public virtual object GetCreateDefaultCollectionItemDto(string collectionExpression)
        {
            var type = RelationshipHelper.GetCollectionExpressionCreateType(collectionExpression, typeof(TUpdateDto));
            return Activator.CreateInstance(type);
        }
        #endregion

        #region TriggerActions

        public virtual Result TriggerAction(object id, ActionDto action, string triggeredBy)
        {

            var entity = Repository.GetById(id);

            if (entity is IEntityAggregateRoot)
            {
                IDomainActionEvent actionEvent = ActionEventsService.CreateEntityActionEvent(action.Action, null, entity, triggeredBy);
                if (actionEvent != null)
                {
                    ((IEntityAggregateRoot)entity).AddActionEvent(actionEvent);

                    var validationResult = Repository.Update(entity, triggeredBy);
                }
            }

            var result = UnitOfWork.Save();

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public async virtual Task<Result> TriggerActionAsync(object id, ActionDto action, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = await Repository.GetByIdAsync(cancellationToken, id);

            if (entity is IEntityAggregateRoot)
            {
                IDomainActionEvent actionEvent = ActionEventsService.CreateEntityActionEvent(action.Action, null, entity, triggeredBy);

                if (actionEvent != null)
                {
                    ((IEntityAggregateRoot)entity).AddActionEvent(actionEvent);

                    var validationResult =Repository.Update(entity, triggeredBy);
                }
            }

            var result = await UnitOfWork.SaveAsync(cancellationToken);

            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.DatabaseValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }
        #endregion

        #region Bulk Trigger Actions
        public virtual List<Result> TriggerActions(BulkDto<ActionDto>[] actions, string triggeredBy)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                try
                {
                    var result = TriggerAction(action.Id, action.Value, triggeredBy);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> TriggerActionsAsync(BulkDto<ActionDto>[] actions, string triggeredBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                try
                {
                    var result = await TriggerActionAsync(action.Id, action.Value, triggeredBy, cancellationToken);
                    results.Add(result);
                }
                catch
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        #endregion
    }
}