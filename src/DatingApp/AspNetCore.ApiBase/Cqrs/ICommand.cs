using AspNetCore.ApiBase.Validation;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Cqrs
{
    public interface ICommand<TResult> : ICommand
    {

    }

    public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
    {
        Task<Result<TResult>> HandleAsync(TCommand command);
    }

    public interface ICommand
    {

    }

    public interface ICommandHandler<TCommand>
    where TCommand : ICommand
    {
        Task<Result> HandleAsync(TCommand command);
    }
}
