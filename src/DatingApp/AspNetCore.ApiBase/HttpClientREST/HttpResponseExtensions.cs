using AspNetCore.ApiBase.HttpClientREST;
using Newtonsoft.Json;
using System.IO;
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

        public static async Task<T> ContentAsTypeStreamAsync<T>(this HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var sr = new StreamReader(stream))
                {
                    using (var tr = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(tr);
                    }
                }
            }
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
            var data = await response.Content.ReadAsStringAsync();

            return data;
        }

        //https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content != null)
                response.Content.Dispose();

            throw new SimpleHttpResponseException(response.StatusCode, response.ReasonPhrase, content);
        }

        //https://stackoverflow.com/questions/21097730/usage-of-ensuresuccessstatuscode-and-handling-of-httprequestexception-it-throws
        public static void EnsureSuccessStatusCode(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.Content != null)
                response.Content.Dispose();

            throw new SimpleHttpResponseException(response.StatusCode, response.ReasonPhrase, content);
        }

    }
}
