using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrabMobile.ApiClient.HttpClientREST
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> ContentAsTypeAsync<T>(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            (serializerSettings != null ? JsonConvert.DeserializeObject<T>(data, serializerSettings) : JsonConvert.DeserializeObject<T>(data));
        }

        public static async Task<string> ContentAsJsonAsync(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = await response.Content.ReadAsStringAsync();
            return serializerSettings != null ? JsonConvert.SerializeObject(data) : JsonConvert.SerializeObject(data, serializerSettings);
        }

        public static async Task<dynamic> ContentAsDynamicAsync(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null)
        {
            var data = await response.Content.ReadAsStringAsync();
            return serializerSettings != null ? JsonConvert.DeserializeObject(data) : JsonConvert.DeserializeObject(data);
        }

        public static async Task<string> ContentAsStringAsync(this HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }
}
