using Common.Exceptions;

namespace Packager.Exceptions
{
    public class DetermineProcessorException : AbstractEngineException
    {
        public DetermineProcessorException(string baseMessage, params object[] parameters) 
            : base(baseMessage, parameters)
        {
        }
    }
}