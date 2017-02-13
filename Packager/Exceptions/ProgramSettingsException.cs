using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;

namespace Packager.Exceptions
{
    public class ProgramSettingsException : AbstractEngineException
    {
        public ProgramSettingsException(string baseMessage, params object[] parameters) : base(baseMessage, parameters)
        {
        }

        public ProgramSettingsException(IEnumerable<string> issues) : 
            base("One or more required settings is missing or invalid:\n  {0}", string.Join("\n  ", issues.Distinct()))
        {
        }
    }
}