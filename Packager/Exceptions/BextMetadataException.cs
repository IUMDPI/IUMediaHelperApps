namespace Packager.Exceptions
{
    public class BextMetadataException : AbstractEngineException
    {
        public BextMetadataException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}