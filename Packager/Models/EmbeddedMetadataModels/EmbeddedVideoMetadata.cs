using Packager.Extensions;
using Packager.Utilities;

namespace Packager.Models.EmbeddedMetadataModels
{
    public class EmbeddedVideoMetadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MasteredDate { get; set; }

        public string Comment { get; set; }

        public ArgumentBuilder AsArguments()
        {
            var arguments = new ArgumentBuilder();
            arguments.Add($"-metadata title={Title.NormalizeForCommandLine().ToQuoted()}");
            arguments.Add($"-metadata date_digitized={MasteredDate.NormalizeForCommandLine().ToQuoted()}");
            arguments.Add($"-metadata comment={Comment.NormalizeForCommandLine().ToQuoted()}");
            arguments.Add($"-metadata description={Description.NormalizeForCommandLine().ToQuoted()}");
            return arguments;
        }
    }
}