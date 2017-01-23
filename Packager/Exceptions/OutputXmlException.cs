using Common.Exceptions;

namespace Packager.Exceptions
{
    public class OutputXmlException : AbstractEngineException
    {
        public OutputXmlException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}