using AspNetCore.ApiBase.Data.Repository;
using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.ApplicationServices
{
    public abstract class ApplicationServiceEntityReadOnlyBase<TEntity, TDto, TUnitOfWork> : ApplicationServiceBase, IApplicationServiceEntityReadOnly<TDto>
          where TEntity : class
          where TDto : class
          where TUnitOfWork : IUnitOfWork
    {
        protected virtual TUnitOfWork UnitOfWork { get; }
        protected virtual IGenericRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();
        protected virtual IActionEventsService ActionEventsService { get; }

        public ApplicationServiceEntityReadOnlyBase(string serviceName, TUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService)
           : base(serviceName, mapper, authorizationService, userService, validationService)
        {
            UnitOfWork = unitOfWork;
            ActionEventsService = actionEventsService;
        }

        public virtual void AddIncludes(List<Expression<Func<TEntity, Object>>> includes)
        {

        }

        public virtual bool IncludeAllCompositionRelationshipProperties => false;
        public virtual bool IncludeAllCompositionAndAggregationRelationshipProperties => false;

        #region GetAll
        public virtual IEnumerable<TDto> GetAll(
        Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
        int? pageNo = null,
        int? pageSize = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.GetAll(orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.GetAllAsync(cancellationToken, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }
        #endregion

        #region Search
        public virtual IEnumerable<TDto> Search(
       string search = "",
       Expression<Func<TDto, bool>> filter = null,
       Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
       int? pageNo = null,
       int? pageSize = null,
       bool includeAllCompositionRelationshipProperties = false,
       bool includeAllCompositionAndAggregationRelationshipProperties = false,
       params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.Search(search, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> SearchAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetSearchCount(
        string search = "",
       Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.GetSearchCount(search, filterConverted);
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.GetSearchCountAsync(cancellationToken, search, filterConverted).ConfigureAwait(false);
        }
        #endregion

        #region Get
        public virtual IEnumerable<TDto> Get(
           Expression<Func<TDto, bool>> filter = null,
           Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
           int? pageNo = null,
           int? pageSize = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.Get(filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.GetAsync(cancellationToken, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetCount(
        Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.GetCount(filterConverted);
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.GetCountAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        #endregion

        #region GetOne
        public virtual TDto GetOne(
          Expression<Func<TDto, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetOne(filterConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetOneAsync(cancellationToken, filterConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetFirst
        public virtual TDto GetFirst(
         Expression<Func<TDto, bool>> filter = null,
         Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetFirst(filterConverted, orderByConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetFirstAsync(cancellationToken, filterConverted, orderByConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetById
        public virtual TDto GetById(object id,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetById(id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken),
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetByIdAsync(cancellationToken, id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);
            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TDto GetByIdWithPagedCollectionProperty(object id,
           string collectionExpression,
           string search = "",
           string orderBy = null,
           bool ascending = false,
           int? pageNo = null,
           int? pageSize = null)
        {
            var bo = Repository.GetByIdWithPagedCollectionProperty(id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null,
            int? pageSize = null)
        {

            var bo = await Repository.GetByIdWithPagedCollectionPropertyAsync(cancellationToken, id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);
            return Mapper.Map<TDto>(bo);
        }

        public int GetByIdWithPagedCollectionPropertyCount(object id,
            string collectionExpression,
            string search = "")
        {
            //var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return Repository.GetByIdWithPagedCollectionPropertyCount(id, collectionExpression, search);
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "")
        {
           // var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return await Repository.GetByIdWithPagedCollectionPropertyCountAsync(cancellationToken, id, collectionExpression, search);
        }
        #endregion

        #region GetByIds
        public virtual IEnumerable<TDto> GetByIds(IEnumerable<object> ids,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = Repository.GetByIds(ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }

        public virtual async Task<IEnumerable<TDto>> GetByIdsAsync(CancellationToken cancellationToken,
         IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = await Repository.GetByIdsAsync(cancellationToken, ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.Exists(filterConverted);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.ExistsAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        public virtual bool Exists(object id)
        {
            return Repository.Exists(id);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            return await Repository.ExistsAsync(cancellationToken, id).ConfigureAwait(false);
        }
        #endregion
    }
}
