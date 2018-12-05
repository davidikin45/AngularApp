using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AspNetCore.ApiBase.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ValidationResult> ValidateObject(object o)
        {
            var context = new ValidationContext(o, _serviceProvider, new Dictionary<object, object>());

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                o,
                context,
               validationResults,
               true);

            return validationResults.Where(r => r != ValidationResult.Success);
        }
    }
}
