namespace AspNetCore.ApiBase.Validation
{
    public enum ErrorType
    {
        UnknownError,
        ObjectDoesNotExist,
        ObjectValidationFailed,
        ConcurrencyConflict,
        EmailSendFailed,
        Unauthorized,
        DatabaseValidationFailed
    }
}
