using Newtonsoft.Json.Linq;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class ActionDto
    {
        public string Action { get; set; }
        public JObject Payload { get; set; }
    }
}