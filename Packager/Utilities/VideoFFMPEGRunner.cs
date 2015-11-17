using Packager.Models;
using Packager.Observers;
using Packager.Providers;

namespace Packager.Utilities
{
    public class VideoFFMPEGRunner : AbstractFFMPEGRunner
    {
        public VideoFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider, IHasher hasher)
            : base(programSettings, processRunner, observers, fileProvider, hasher)
        {
            ProdOrMezzArguments = programSettings.FFMPEGVideoMezzanineArguments;
            AccessArguments = programSettings.FFMPEGAudioAccessArguments;
        }

        protected override string NormalizingArguments => "-acodec copy -vcodec copy";

        public override string ProdOrMezzArguments { get; }
        public override string AccessArguments { get; }
    }
}