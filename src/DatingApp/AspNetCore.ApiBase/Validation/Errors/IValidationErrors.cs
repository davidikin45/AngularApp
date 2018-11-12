using System.Collections.Generic;

namespace AspnetCore.ApiBase.Validation.Errors
{
    public interface IValidationErrors
    {
        List<IError> Errors { get; set; }
    }
}
