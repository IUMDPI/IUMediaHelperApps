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
        }

        public Task<ObjectFileModel> CreateMezzanineDerivative(ObjectFileModel original, ObjectFileModel target, VideoPodMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            throw new NotImplementedException();
        }

        public Task Normalize(ObjectFileModel original, BextMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public Task Verify(List<ObjectFileModel> originals)
        {
            throw new NotImplementedException();
        }

        public string MezzanineArguments { get; }
        public string AccessArguments { get; }
    }
}