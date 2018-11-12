using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Tasks
{
    public interface IRunAfterApplicationConfiguration
    {
        void Execute();
    }
}