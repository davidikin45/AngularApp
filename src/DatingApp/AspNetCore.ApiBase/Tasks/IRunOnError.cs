using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Tasks
{
    public interface IRunOnError
	{
        Task ExecuteAsync();
    }
}