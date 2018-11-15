using Newtonsoft.Json;
using System.Net.Http;

namespace GrabMobile.ApiClient.HttpClientREST
{
    public static class HttpResponseExtensions
    {
        public static T ContentAsType<T>(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            (serializerSettings != null ? JsonConvert.DeserializeObject<T>(data, serializerSettings) : JsonConvert.DeserializeObject<T>(data));
        }

        public static string ContentAsJson(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return serializerSettings != null ? JsonConvert.SerializeObject(data) : JsonConvert.SerializeObject(data, serializerSettings);
        }

        public static dynamic ContentAsDynamic(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return serializerSettings != null ? JsonConvert.DeserializeObject(data) : JsonConvert.DeserializeObject(data);
        }

        public static string ContentAsString(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
