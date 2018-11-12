using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Tasks
{
    public interface IRunOnEachRequest
	{
		Task ExecuteAsync();
	}
}