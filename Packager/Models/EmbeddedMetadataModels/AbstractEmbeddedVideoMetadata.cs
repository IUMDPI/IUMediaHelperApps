using Packager.Extensions;
using Packager.Utilities.Process;

namespace Packager.Models.EmbeddedMetadataModels
{
    public abstract class AbstractEmbeddedVideoMetadata : AbstractEmbeddedMetadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MasteredDate { get; set; }
        public string Comment { get; set; }

        public override ArgumentBuilder AsArguments()
        {
            var arguments = new ArgumentBuilder
            {
                $"-metadata title={Title.NormalizeForCommandLine().ToQuoted()}",
                $"-metadata comment={Comment.NormalizeForCommandLine().ToQuoted()}",
                $"-metadata description={Description.NormalizeForCommandLine().ToQuoted()}"
            };
            return arguments;
        }
    }
}