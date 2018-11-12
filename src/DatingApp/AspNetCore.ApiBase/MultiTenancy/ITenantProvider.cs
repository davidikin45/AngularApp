namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantProvider<TTentant>
        where TTentant : AppTenant
    {
        TTentant GetTenant();
    }
}
