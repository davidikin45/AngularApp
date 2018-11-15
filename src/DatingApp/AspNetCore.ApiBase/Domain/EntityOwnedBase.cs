using System;

namespace AspNetCore.ApiBase.Domain
{
    public abstract class EntityOwnedBase<T> : EntityBase<T>, IEntityOwned where T : IEquatable<T>
    {
        public string OwnedBy { get; set; }
    }
}
