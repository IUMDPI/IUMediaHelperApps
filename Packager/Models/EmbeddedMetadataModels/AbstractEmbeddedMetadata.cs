using Packager.Utilities;
using Packager.Utilities.ProcessRunners;

namespace Packager.Models.EmbeddedMetadataModels
{
    public abstract class AbstractEmbeddedMetadata
    {
        public abstract ArgumentBuilder AsArguments();
    }
}