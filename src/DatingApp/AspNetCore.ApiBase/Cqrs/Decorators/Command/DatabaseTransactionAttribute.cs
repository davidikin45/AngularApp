using System;

namespace AspNetCore.ApiBase.Cqrs.Decorators.Command
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DatabaseTransactionAttribute : Attribute
    {
        public DatabaseTransactionAttribute()
        {
        }
    }
}
