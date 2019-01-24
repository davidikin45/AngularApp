using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.ApiBase.Controllers.ApiClient
{
    public abstract class ApiClientBase : IApiClient
    {
        protected readonly HttpClient httpClient;
        protected readonly JsonSerializerSettings settings;

        protected readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public ApiClientBase(HttpClient httpClient)
        {
            settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.DateParseHandling = DateParseHandling.DateTime;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            this.httpClient = httpClient;
            InitializeRepositories(httpClient);
        }

        public abstract void InitializeRepositories(HttpClient httpClient);

        public void AddRepository<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>(IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> repository)
            where TCreateDto : class
            where TReadDto : class
            where TUpdateDto : class
            where TDeleteDto : class
        {
            var key = typeof(IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>);
            repositories[key] = repository;
        }

        public IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> Repository<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>()
            where TCreateDto : class
            where TReadDto : class
            where TUpdateDto : class
            where TDeleteDto : class
        {
            var key = typeof(IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>);
            return (IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>)repositories[key];
        }
    }
}
