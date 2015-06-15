namespace Packager.Exceptions
{
    internal class OutputXmlException : AbstractEngineException
    {
        public OutputXmlException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}