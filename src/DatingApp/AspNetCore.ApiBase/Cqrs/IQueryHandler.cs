﻿using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Cqrs
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
