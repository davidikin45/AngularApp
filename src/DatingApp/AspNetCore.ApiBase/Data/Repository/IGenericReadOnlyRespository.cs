using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.Repository
{
    public interface IGenericReadOnlyRepository<TEntity>
         where TEntity : class
    {
        IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters);
        Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters);

        IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllAsync(
                CancellationToken cancellationToken,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                int? skip = null,
                int? take = null,
                bool includeAllCompositionRelationshipProperties = false,
                bool includeAllCompositionAndAggregationRelationshipProperties = false,
                params Expression<Func<TEntity, Object>>[] includeProperties)
                ;

        IEnumerable<TEntity> GetAllNoTracking(
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllNoTrackingAsync(
                 CancellationToken cancellationToken,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                int? skip = null,
                int? take = null,
                bool includeAllCompositionRelationshipProperties = false,
                bool includeAllCompositionAndAggregationRelationshipProperties = false,
                params Expression<Func<TEntity, Object>>[] includeProperties)
                ;

        IEnumerable<TEntity> Search(
             string search = "",
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           int? skip = null,
           int? take = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> SearchAsync(
            CancellationToken cancellationToken,
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> SearchNoTracking(
         string search = "",
       Expression<Func<TEntity, bool>> filter = null,
       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
       int? skip = null,
       int? take = null,
       bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
       params Expression<Func<TEntity, Object>>[] includeProperties)
       ;

        Task<IEnumerable<TEntity>> SearchNoTrackingAsync(
            CancellationToken cancellationToken,
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TEntity>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> GetNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<IEnumerable<TEntity>> GetNoTrackingAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetOneNoTracking(
        Expression<Func<TEntity, bool>> filter = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        ;

        Task<TEntity> GetOneNoTrackingAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetFirstNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<TEntity> GetFirstNoTrackingAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetById(object id, bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetByIdAsync(
            CancellationToken cancellationToken,
            object id,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;


        TEntity GetByIdNoTracking(
            object id,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<TEntity> GetByIdNoTrackingAsync(CancellationToken cancellationToken,
            object id,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetByIdWithPagedCollectionProperty(object id,
      string collectionExpression,
      string search = "",
      string orderBy = null,
      bool ascending = false,
      int? skip = null,
      int? take = null);

        Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null,
            int? take = null);

        int GetByIdWithPagedCollectionPropertyCount(object id,
            string collectionExpression,
            string search = "");

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "");

        IEnumerable<TEntity> GetByIds(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> GetByIdsAsync(CancellationToken cancellationToken,
            IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> GetByIdsNoTracking(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
       ;

        Task<IEnumerable<TEntity>> GetByIdsNoTrackingAsync(CancellationToken cancellationToken,
            IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        int GetCount(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<int> GetCountAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null)
            ;

        int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
          ;

        Task<int> GetSearchCountAsync(CancellationToken cancellationToken,
            string search = "", Expression<Func<TEntity, bool>> filter = null)
            ;


        bool Exists(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<bool> ExistsAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null)
            ;

        bool ExistsNoTracking(Expression<Func<TEntity, bool>> filter = null)
          ;

        Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null)
            ;

        bool Exists(object id)
         ;

        Task<bool> ExistsAsync(CancellationToken cancellationToken, object id)
           ;

        bool ExistsNoTracking(object id)
        ;

        Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, object id)
           ;

        bool ExistsById(object id)
        ;

        Task<bool> ExistsByIdAsync(CancellationToken cancellationToken,
            object id)
            ;

        bool ExistsByIdNoTracking(object id)
          ;

        Task<bool> ExistsByIdNoTrackingAsync(CancellationToken cancellationToken,
            object id)
            ;

    }
}
