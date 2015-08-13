using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    internal class StandardValidatorCollection : HashSet<IValidator>, IValidatorCollection
    {
        public ValidationResults Validate(object instance)
        {
            var results = ValidateProperties(instance);
            results.AddRange(ValidateChildObjects(instance));

            return results;
        }

        private ValidationResults ValidateProperties(object instance)
        {
            var results = new ValidationResults();
            foreach (var property in GetPropertiesToValidate<PropertyValidationAttribute>(instance))
            {
                foreach (var attribute in property.GetCustomAttributes<PropertyValidationAttribute>())
                {
                    var validator = this.SingleOrDefault(v => v.Supports == attribute.GetType());
                    if (validator == null)
                    {
                        results.Add(new ValidationResult("Could not find validator to support {0}", attribute.GetType()));
                        continue;
                    }

                    results.Add(validator.Validate(property.GetValue(instance), property.Name));
                }
            }

            return results;
        }

        private ValidationResults ValidateChildObjects(object instance)
        {
            var results = new ValidationResults();
            foreach (var property in GetPropertiesToValidate<ValidateObjectAttribute>(instance))
            {
                var value = property.GetValue(instance);

                if (instance == null)
                {
                    results.Add(new ValidationResult("Value not set for {0}", property.Name));
                    continue;
                }

                results.AddRange(Validate(value));
            }

            return results;
        }

        private IEnumerable<PropertyInfo> GetPropertiesToValidate<T>(object instance) where T : Attribute
        {
            return instance.GetType().GetProperties().Where(p => p.GetCustomAttributes<T>().Any());
        }
    }
}