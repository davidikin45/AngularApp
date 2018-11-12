using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Tasks
{
    public interface IRunAfterEachRequest
	{
        Task ExecuteAsync();
    }
}