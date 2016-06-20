using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Hashing;

namespace Packager.Utilities.ProcessRunners
{
    public class VideoFFMPEGRunner : AbstractFFMPEGRunner
    {
        private IMediaInfoProvider MediaInfoProvider { get; }

        private const string ReduceStreamsAccessArguments =
            "-filter_complex \"[0:a:0][0:a:1]amerge=inputs=2[aout]\" -map 0:v -map \"[aout]\"";

        public VideoFFMPEGRunner(IProgramSettings programSettings, IFileProvider fileProvider, IHasher hasher, IObserverCollection observers, IProcessRunner processRunner, IMediaInfoProvider mediaInfoProvider) : base(programSettings, fileProvider, hasher, observers, processRunner)
        {
            MediaInfoProvider = mediaInfoProvider;
            ProdOrMezzArguments = programSettings.FFMPEGVideoMezzanineArguments;
            AccessArguments = programSettings.FFMPEGVideoAccessArguments;
        }

        protected override string NormalizingArguments => "-map 0 -acodec copy -vcodec copy";

        public override string ProdOrMezzArguments { get; }
        public override string AccessArguments { get; }

        public override async Task<AbstractFile> CreateAccessDerivative(AbstractFile original,
            CancellationToken cancellationToken)
        {
            var notes = new List<string>();
            var arguments = new ArgumentBuilder(AccessArguments);

            var mediaInfo = await MediaInfoProvider.GetMediaInfo(original, cancellationToken);
            if (mediaInfo.AudioStreams > 1)
            {
                notes.Add("Multiple audio streams present; merging audio streams.");
                arguments.AddArguments(ReduceStreamsAccessArguments);
            }

            return await CreateDerivative(original, new AccessFile(original), arguments, cancellationToken, notes);
        }
    }
}