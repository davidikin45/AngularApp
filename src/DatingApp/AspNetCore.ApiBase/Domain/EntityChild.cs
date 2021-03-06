﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore.ApiBase.Domain
{
    public abstract class EntityChild<T> : EntityBase<T>, IEntityChild where T : IEquatable<T>
    {
        [NotMapped]
        public TrackingState TrackingState { get; set; }
    }
}
