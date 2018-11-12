namespace AspNetCore.ApiBase.Features
{
    public interface IFeatureService
    {
        bool IsFeatureActive(string featureName);
    }
}
