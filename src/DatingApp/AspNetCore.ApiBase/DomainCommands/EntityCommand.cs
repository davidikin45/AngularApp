namespace AspNetCore.ApiBase.DomainCommands
{
    public abstract class EntityCommand<T> : IDomainCommand where T : class
    {
        public T Entity { get; }
        public string TriggeredBy { get; }

        public EntityCommand(T entity, string triggeredBy)
        {
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
