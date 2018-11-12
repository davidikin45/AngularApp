namespace AspNetCore.ApiBase.Domain
{
    public interface IEntityConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
