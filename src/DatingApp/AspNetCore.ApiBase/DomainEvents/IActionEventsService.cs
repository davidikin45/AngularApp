namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IActionEventsService
    {
        bool IsValidAction<T>(string action);
        IDomainActionEvent CreateEntityActionEvent(string action, dynamic args, object entity, string triggeredBy);
    }
}
