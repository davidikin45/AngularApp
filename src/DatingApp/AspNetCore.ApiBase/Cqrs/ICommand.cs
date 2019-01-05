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
}
