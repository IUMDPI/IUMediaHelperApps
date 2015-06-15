namespace Packager.Exceptions
{
    public class AddMetadataException : AbstractEngineException
    {
        public AddMetadataException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}