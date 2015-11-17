using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
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
            var metadata = await GetMetadata<VideoPodMetadata>(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<ObjectFileModel>().Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();
            
            // temporary


            return filesToProcess;
        }

        private async Task<List<ObjectFileModel>> CreateMezzanineDerivatives(List<ObjectFileModel> models, AudioPodMetadata podMetadata)
        {
            var results = new List<ObjectFileModel>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = master.ToProductionFileModel();
                //var metadata = null;// AudioMetadataFactory.Generate(models, derivative, podMetadata);
                //results.Add(await FFPMpegRunner.CreateProductionDerivative(master, derivative, metadata));
            }

            return results;
        }

        private async Task<List<ObjectFileModel>> CreateAccessDerivatives(IEnumerable<ObjectFileModel> models)
        {
            var results = new List<ObjectFileModel>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsProductionVersion()))
            {
                results.Add(await FFPMpegRunner.CreateAccessDerivative(model));
            }
            return results;
        }
    }
}