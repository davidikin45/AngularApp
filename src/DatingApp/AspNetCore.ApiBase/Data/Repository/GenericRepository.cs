using AspNetCore.ApiBase.Data.Helpers;
using AspNetCore.ApiBase.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.Repository
{
    public class GenericRepository<TEntity> : GenericReadOnlyRepository<TEntity>, IGenericRepository<TEntity>
   where TEntity : class
    {
        public GenericRepository(DbContext context)
            : base(context)
        {
        }

        #region Insert
        public virtual TEntity Add(TEntity entity, string addedBy)
        {
            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.CreatedOn = DateTime.UtcNow;
                auditableEntity.CreatedBy = addedBy;
            }

            var ownedEntity = entity as IEntityOwned;
            if (ownedEntity != null)
            {
                ownedEntity.OwnedBy = addedBy;
            }

            return context.AddEntity(entity);
        }
        #endregion

        #region Update
        public virtual TEntity Update(TEntity entity, string updatedBy)
        {
            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.UpdatedOn = DateTime.UtcNow;
                auditableEntity.UpdatedBy = updatedBy;
            }

            return context.UpdateEntity(entity);
        }
        #endregion

        #region Delete
        public virtual void Delete(object id, string deletedBy)
        {
            TEntity entity = GetById(id); // For concurrency purposes need to get latest version
            Delete(entity, deletedBy);
        }

        public virtual void Delete(TEntity entity, string deletedBy)
        {
            context.RemoveEntity(entity);
        }
        #endregion

        #region Save Changes
        public virtual Task<bool> SaveAsync()
        {
            return SaveAsync(CancellationToken.None);
        }

        public virtual async Task<bool> SaveAsync(CancellationToken cancellationToken)
        {
            return (await context.SaveChangesAsync(cancellationToken) >= 0);
        }
        #endregion
    }
}
