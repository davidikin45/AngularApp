using AspNetCore.ApiBase.Validation;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Cqrs
{
    public interface ICqrsMediator
    {
        Task<Result> DispatchAsync(ICommand command);
        Task<Result<T>> DispatchAsync<T>(ICommand<T> command);

        Task<T> DispatchAsync<T>(IQuery<T> query);
    }
}
