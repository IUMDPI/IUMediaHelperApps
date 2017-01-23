using Common.Exceptions;

namespace Packager.Exceptions
{
    public class ResolverException : AbstractEngineException
    {
        public ResolverException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}