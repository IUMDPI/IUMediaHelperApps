using Common.Exceptions;

namespace Packager.Exceptions
{
    public class GenerateDerivativeException : AbstractEngineException
    {
        public GenerateDerivativeException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}