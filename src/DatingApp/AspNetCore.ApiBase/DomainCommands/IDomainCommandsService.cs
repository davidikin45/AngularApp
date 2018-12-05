using Newtonsoft.Json.Linq;

namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IDomainCommandsService
    {
        bool IsValidAction<T>(string action);
        IDomainCommand CreateEntityActionEvent(string action, JObject payload, object entity, string triggeredBy);
    }
}
