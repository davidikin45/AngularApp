using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AspNetCore.ApiBase.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEnumerable<ValidationResult> ValidateObject(object o)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = new ValidationContext(o, scope.ServiceProvider, new Dictionary<object, object>());

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
}
