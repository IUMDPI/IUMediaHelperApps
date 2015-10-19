using System;

namespace Packager.Exceptions
{
    internal class NormalizeOriginalException : AbstractEngineException
    {
        public NormalizeOriginalException(string baseMessage, params object[] parameters) : base(baseMessage, parameters)
        {
        }

        public NormalizeOriginalException(Exception innerException, string baseMessage, params object[] parameters) : base(innerException, baseMessage, parameters)
        {
        }
    }
}