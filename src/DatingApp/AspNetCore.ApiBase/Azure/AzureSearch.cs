﻿using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.AzureStorage
{
    public static class AzureSearchExtensions
    {
        public static IServiceCollection AddAzureSearch(this IServiceCollection services, Func<IServiceProvider, IAzureSearch> implementationFactory)
        {
            return services.AddTransient(implementationFactory);
        }
    }

    public abstract class AzureSearch : IAzureSearch
    {
        private readonly SearchIndexClient _searchIndexClient;

        public AzureSearch(string searchServiceName, string indexName, string queryApiKey)
        {
            _searchIndexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
        }

        public Task<DocumentSearchResult> SearchAsync(string searchText)
        {
            return _searchIndexClient.Documents.SearchAsync(searchText);
        }

        public Task<DocumentSearchResult> SearchAsync(string searchText, string tag, string value)
        {
            SearchParameters parameters = new SearchParameters()
            {
                Filter = $"{tag} eq '{value}'",
                QueryType = QueryType.Full
            };

            return _searchIndexClient.Documents.SearchAsync(searchText, parameters);
        }
    }
}
