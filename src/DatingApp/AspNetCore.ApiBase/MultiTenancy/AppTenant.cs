namespace AspNetCore.ApiBase.MultiTenancy
{
    public class AppTenant
    {
        public string Name { get; set; }
        public string[] HostNames { get; set; }
        public string ConnectionString { get; set; }
        public bool Active { get; set; }
    }
}
