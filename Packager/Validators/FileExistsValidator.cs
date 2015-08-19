using System;
using Packager.Extensions;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Validators
{
    public class FileExistsValidator : IValidator
    {
        public FileExistsValidator(IFileProvider fileProvider)
        {
            FileProvider = fileProvider;
        }

        private IFileProvider FileProvider { get; set; }

        public ValidationResult Validate(object value, string friendlyName)
        {
            return FileProvider.FileExists(value.ToDefaultIfEmpty())
                ? ValidationResult.Success
                : new ValidationResult("Invalid path specified for {0}: {1}", friendlyName, value.ToDefaultIfEmpty("[not set]"));
        }

        public Type Supports => typeof (ValidateFileAttribute);
    }
}