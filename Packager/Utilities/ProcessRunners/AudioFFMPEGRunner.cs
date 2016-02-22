using System.Threading;
using Packager.Extensions;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Hashing;

namespace Packager.Utilities.ProcessRunners
{
    // ReSharper disable once InconsistentNaming
    public class AudioFFMPEGRunner : AbstractFFMPEGRunner
    {
        private const string RequiredProductionBextArguments = "-write_bext 1";
        private const string RequiredProductionRiffArguments = "-rf64 auto";

        public AudioFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider, IHasher hasher, CancellationToken cancellationToken) :
            base(programSettings, processRunner, observers, fileProvider, hasher, cancellationToken)
        {
            AccessArguments = programSettings.FFMPEGAudioAccessArguments;
            ProdOrMezzArguments = AddRequiredArguments(programSettings.FFMPEGAudioProductionArguments);
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