using System;
using System.Collections;
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
            var result = ValidateAsEnumerable(value, friendlyName);
            return result.Result
                ? result
                : ValidateAsDictionary(value, friendlyName);
        }

        private static ValidationResult ValidateAsEnumerable(object value, string friendlyName)
        {
            var enumerable = value as IEnumerable<object>;
            if (enumerable == null)
            {
                return new ValidationResult("{0} must have at least one member", friendlyName);
            }

            return enumerable.Any()
                ? ValidationResult.Success
                : new ValidationResult("{0} must have at least one member", friendlyName);
        }

        private static ValidationResult ValidateAsDictionary(object value, string friendlyName)
        {
            var dictionary = value as IDictionary;
            if (dictionary == null)
            {
                return new ValidationResult("{0} must have at least one member", friendlyName);
            }

            return dictionary.Count>0
                ? ValidationResult.Success
                : new ValidationResult("{0} must have at least one member", friendlyName);
        }
    }
}