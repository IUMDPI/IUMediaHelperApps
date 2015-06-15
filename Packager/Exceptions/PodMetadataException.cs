namespace Packager.Exceptions
{
    public class PodMetadataException : AbstractEngineException
    {
        public PodMetadataException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}