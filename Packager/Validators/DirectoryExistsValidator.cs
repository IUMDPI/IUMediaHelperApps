using System;
using Packager.Extensions;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    public class DirectoryExistsValidator : IValidator
    {
        public DirectoryExistsValidator(IDirectoryProvider directoryProvider)
        {
            DirectoryProvider = directoryProvider;
        }

        private IDirectoryProvider DirectoryProvider { get; set; }

        public ValidationResult Validate(object value, string friendlyName)
        {
            return DirectoryProvider.DirectoryExists(value.ToDefaultIfEmpty())
                ? ValidationResult.Success
                : new ValidationResult("Invalid path specified for {0}: {1}", friendlyName, value.ToDefaultIfEmpty("[not set]"));
        }

        public Type Supports => typeof (ValidateFolderAttribute);
    }
}