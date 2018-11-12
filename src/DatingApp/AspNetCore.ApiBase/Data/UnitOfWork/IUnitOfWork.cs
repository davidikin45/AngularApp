using AspNetCore.ApiBase.Data.Repository;
using AspNetCore.ApiBase.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Result<int> Save();
        Task<Result<int>> SaveAsync();
        Task<Result<int>> SaveAsync(CancellationToken cancellationToken);
    }
}
