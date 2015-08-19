using System;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    public class UriValidator : IValidator
    {
        public ValidationResult Validate(object value, string friendlyName)
        {
            return Uri.IsWellFormedUriString(value.ToDefaultIfEmpty(), UriKind.Absolute)
                ? ValidationResult.Success
                : new ValidationResult("Invalid uri specified for {0}: {1}", friendlyName, value.ToDefaultIfEmpty("[not set]"));
        }

        public Type Supports => typeof (ValidateUriAttribute);
    }
}