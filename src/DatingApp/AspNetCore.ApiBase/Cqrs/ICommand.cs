using AspNetCore.ApiBase.Validation;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Cqrs
{
    public abstract class UserCommand<T> : ICommand<T>
    {
        public string User { get; }

        public UserCommand(string user)
        {
            User = user;
        }
    }

    public interface ICommand<TResult> : ICommand
    {

    }

    public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
    {
        Task<Result<TResult>> HandleAsync(TCommand command);
    }

    public abstract class UserCommand : ICommand
    {
        public string User { get; }

        public UserCommand(string user)
        {
            User = user;
        }
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
