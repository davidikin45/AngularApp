﻿using AspNetCore.ApiBase.DomainEvents;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Domain
{
    public abstract class EntityAggregateRootBase<T> : EntityBase<T>, IEntityAggregateRoot, IEntityConcurrencyAware where T : IEquatable<T>
    {
        //Optimistic Concurrency. Potentially ETags serve the same purpose
        public byte[] RowVersion { get; set; }
    }
}
