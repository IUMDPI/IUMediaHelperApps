namespace Packager.Exceptions
{
    public class DeserializeValueException : AbstractEngineException
    {
        public DeserializeValueException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}