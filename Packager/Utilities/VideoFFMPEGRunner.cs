using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packager.Models;
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

        public async Task<ObjectFileModel> CreateMezzanineDerivative(ObjectFileModel original, ObjectFileModel target)
        {
            if (TargetAlreadyExists(target))
            {
                LogAlreadyExists(target);
                return target;
            }

            var args = new ArgumentBuilder(MezzanineArguments);
            
            return await CreateDerivative(original, target, args);
        }

        public async Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            return await CreateDerivative(original, original.ToAudioAccessFileModel(), new ArgumentBuilder(AccessArguments));
        }

        public async Task EmbedMetadata(List<ObjectFileModel> fileModels, VideoPodMetadata metadata)
        {
            foreach (var model in fileModels.Where(m => m.IsMezzanineVersion() || m.IsPreservationIntermediateVersion() || m.IsPreservationVersion()))
            {
                
            }
        }

        public string MezzanineArguments { get; }
        public string AccessArguments { get; }
    }
}