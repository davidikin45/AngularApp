using AspNetCore.ApiBase.DomainEvents;
using Newtonsoft.Json.Linq;

namespace AspNetCore.ApiBase.DomainCommands
{
    public class EntityCommand<T> : IDomainCommand where T : class
    {
        public string Action { get; }
        public JObject Payload { get; }
        public T Entity { get; }
        public string TriggeredBy { get; }

        public EntityCommand(string action, JObject payload, T entity, string triggeredBy)
        {
            Action = action;
            Payload = payload;
            Entity = entity;
            TriggeredBy = triggeredBy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityCommand<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity);
        }
    }
}
