using System;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    public class ValueRequiredValidator : IValidator
    {
        public ValidationResult Validate(object value, string friendlyName)
        {
            return value.ToDefaultIfEmpty().IsSet()
                ? ValidationResult.Success
                : new ValidationResult("Value not set for {0}", friendlyName);
        }

        public Type Supports => typeof (RequiredAttribute);
    }
}