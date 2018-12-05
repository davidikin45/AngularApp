﻿using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Cqrs
{
    public abstract class UserQuery<TResult> : IQuery<TResult>
    {
        public string User { get; }

        public UserQuery(string user)
        {
            User = user;
        }
    }

    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
