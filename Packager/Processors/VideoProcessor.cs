using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;
using Packager.Providers;

namespace Packager.Processors
{
    internal class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
        }

        protected override string ProductionFileExtension => ".mkv";
        protected override string AccessFileExtension => ".mp4";
        protected override string MezzanineFileExtension => ".mkv";
        protected override string PreservationFileExtension => ".mkv";
        protected override string PreservationIntermediateFileExtenstion => ".mkv";

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<ConsolidatedVideoPodMetadata>(filesToProcess);

            // temporary
            return filesToProcess;
        }
    }
}