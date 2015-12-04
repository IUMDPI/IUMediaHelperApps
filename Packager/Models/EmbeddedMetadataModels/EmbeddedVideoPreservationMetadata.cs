using Packager.Extensions;
using Packager.Utilities;
using Packager.Utilities.Process;

namespace Packager.Models.EmbeddedMetadataModels
{
    public class EmbeddedVideoPreservationMetadata:AbstractEmbeddedVideoMetadata
    {
        public override ArgumentBuilder AsArguments()
        {
            var arguments = base.AsArguments();
            arguments.Add($"-metadata date_digitized={MasteredDate.NormalizeForCommandLine().ToQuoted()}");
            return arguments;
        }
    }
}