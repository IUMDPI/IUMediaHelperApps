using System;
using System.Linq;
using Packager.Validators;

namespace Packager.Exceptions
{
    public class PodMetadataException : AbstractEngineException
    {
        public PodMetadataException(Exception innerException, string baseMessage, params object[] parameters)
            : base(innerException, baseMessage, parameters)
        {
        }

        public PodMetadataException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }

        public PodMetadataException(ValidationResults validationResults) : base("One or more required metadata properties is missing or invalid:\n  {0}", string.Join("\n  ", validationResults.Issues.Distinct()))
        {
        }
    }
}