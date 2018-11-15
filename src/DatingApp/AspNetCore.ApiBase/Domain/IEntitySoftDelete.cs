using System;

namespace AspNetCore.ApiBase.Domain
{
    public interface IEntitySoftDelete
    {
       bool IsDeleted { get; set; }
       DateTime? DeletedOn { get; set; }
       string DeletedBy { get; set; }
    }
}
