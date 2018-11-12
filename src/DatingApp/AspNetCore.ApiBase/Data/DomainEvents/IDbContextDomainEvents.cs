using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.DomainEvents
{
    public interface IDbContextDomainEvents
    {
        Task FirePreCommitEventsAsync();
        Task FirePostCommitEventsAsync();

        IEnumerable<object> GetNewDeletedEntities();
        IEnumerable<object> GetNewUpdatedEntities();
        IEnumerable<object> GetNewInsertedEntities();

        IEnumerable<object> GetPreCommittedDeletedEntities();
        IEnumerable<object> GetPreCommittedUpdatedEntities();
        IEnumerable<object> GetPreCommittedInsertedEntities();
    }
}
