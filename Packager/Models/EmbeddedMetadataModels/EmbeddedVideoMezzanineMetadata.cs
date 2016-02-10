using Packager.Extensions;
using Packager.Utilities.Process;

namespace Packager.Models.EmbeddedMetadataModels
{
    public class EmbeddedVideoMezzanineMetadata : AbstractEmbeddedVideoMetadata
    {
        public override ArgumentBuilder AsArguments()
        {
            var arguments = base.AsArguments();
            arguments.Add($"-metadata date={MasteredDate.NormalizeForCommandLine().ToQuoted()}");
            return arguments;
        }
    }
}