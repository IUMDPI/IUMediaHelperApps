using Packager.Extensions;
using Packager.Providers;

namespace Packager.Utilities.ProcessRunners
{
    // ReSharper disable once InconsistentNaming
    public class AudioFFMPEGRunner : AbstractFFMPEGRunner
    {
        private const string RequiredProductionBextArguments = "-write_bext 1";
        private const string RequiredProductionRiffArguments = "-rf64 auto";

        public AudioFFMPEGRunner(IDependencyProvider dependencyProvider) :
                base(dependencyProvider)
        {
            AccessArguments = dependencyProvider.ProgramSettings.FFMPEGAudioAccessArguments;
            ProdOrMezzArguments = AddRequiredArguments(dependencyProvider.ProgramSettings.FFMPEGAudioProductionArguments);
        }

        protected override string NormalizingArguments => "-acodec copy -write_bext 1 -rf64 auto -map_metadata -1";
        public override string ProdOrMezzArguments { get; }
        public override string AccessArguments { get; }

        private static string AddRequiredArguments(string arguments)
        {
            if (arguments.IsNotSet())
            {
                return arguments;
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionBextArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionBextArguments}";
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionRiffArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionRiffArguments}";
            }

            return arguments;
        }
    }
}