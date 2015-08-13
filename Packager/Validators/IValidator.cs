using System;

namespace Packager.Validators
{
    public interface IValidator
    {
        Type Supports { get; }
        ValidationResult Validate(object value, string friendlyName);
    }
}