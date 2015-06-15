using System;

namespace Packager.Exceptions
{
    public abstract class AbstractEngineException : Exception
    {
        protected AbstractEngineException(string baseMessage, params object[] parameters)
            : base(string.Format(baseMessage, parameters))
        {
        }
    }
}