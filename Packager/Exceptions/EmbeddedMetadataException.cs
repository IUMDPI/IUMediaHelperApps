using Common.Exceptions;

namespace Packager.Exceptions
{
    public class EmbeddedMetadataException : AbstractEngineException
    {
        public EmbeddedMetadataException(string baseMessage, params object[] parameters)
            : base(baseMessage, parameters)
        {
        }
    }
}