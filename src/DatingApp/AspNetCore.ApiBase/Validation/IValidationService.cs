using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.ApiBase.Validation
{
    public interface IValidationService
    {
        IEnumerable<ValidationResult> ValidateObject(object o);
    }
}
