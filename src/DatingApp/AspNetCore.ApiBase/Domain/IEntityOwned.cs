namespace AspNetCore.ApiBase.Domain
{
    public interface IEntityOwned
    {
        string OwnedBy { get; set; }
    }
}
