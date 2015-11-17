using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Providers;

namespace Packager.Utilities
{
    public class VideoFFMPEGRunner : AbstractFFMPEGRunner, IVideoFFMPEGRunner
    {
        public VideoFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider)
            : base(programSettings, processRunner, observers, fileProvider)
        {
            MezzanineArguments = programSettings.FFMPEGVideoMezzanineArguments;
            AccessArguments = programSettings.FFMPEGAudioAccessArguments;
        }

        public async Task<ObjectFileModel> CreateMezzanineDerivative(ObjectFileModel original, ObjectFileModel target, VideoPodMetadata metadata)
        {
            if (TargetAlreadyExists(target))
            {
                LogAlreadyExists(target);
                return target;
            }

            var args = new ArgumentBuilder(MezzanineArguments);
            //.AddArguments(metadata.AsArguments());

            return await CreateDerivative(original, target, args);
        }

        public async Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            return await CreateDerivative(original, original.ToAudioAccessFileModel(), new ArgumentBuilder(AccessArguments));
        }

        public async Task Normalize(ObjectFileModel original, BextMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public async Task Verify(List<ObjectFileModel> originals)
        {
            throw new NotImplementedException();
        }

        public string MezzanineArguments { get; }
        public string AccessArguments { get; }
    }
}