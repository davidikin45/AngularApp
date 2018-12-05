using System;

namespace AspNetCore.ApiBase.Cqrs.Decorators.Command
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CacheQueryAttribute : Attribute
    {
        public CacheQueryAttribute()
        {
        }
    }
}
