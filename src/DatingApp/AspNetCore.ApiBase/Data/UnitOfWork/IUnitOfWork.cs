using AspNetCore.ApiBase.Data.Repository;
using AspNetCore.ApiBase.Validation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        bool AutoDetectChangesEnabled { get; set; }
        QueryTrackingBehavior QueryTrackingBehavior { get; set; }

        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Result<int> Complete();
        Task<Result<int>> CompleteAsync();
        Task<Result<int>> CompleteAsync(CancellationToken cancellationToken);
    }
}
