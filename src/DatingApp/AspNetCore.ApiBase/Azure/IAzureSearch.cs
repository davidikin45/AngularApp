using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.AzureStorage
{
    public interface IAzureSearch
    {
        Task<DocumentSearchResult> SearchAsync(string searchTerm);
    }
}
