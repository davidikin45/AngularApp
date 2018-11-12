using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Tasks
{
    public interface IAsyncInitializer
    {
        Task ExecuteAsync();
    }
}