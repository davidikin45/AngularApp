namespace AspnetCore.ApiBase.Validation.Errors
{
    public interface IError
    {
        string PropertyName { get; }
        string PropertyExceptionMessage { get; }
    }
}
