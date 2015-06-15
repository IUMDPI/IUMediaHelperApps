namespace Packager.Exceptions
{
    public class LookupException : AbstractEngineException
    {
        public LookupException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}