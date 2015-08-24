using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    public class MembersValidator : IValidator
    {
        public Type Supports => typeof (HasMembersAttribute);

        public ValidationResult Validate(object value, string friendlyName)
        {
            var enumerable = value as IEnumerable<object>;
            if (enumerable == null)
            {
                return new ValidationResult("{0} must have at least one member", friendlyName);
            }

            return (enumerable.Any())
                ? ValidationResult.Success
                : new ValidationResult("{0} must have at least one member", friendlyName);
        }
    }
}