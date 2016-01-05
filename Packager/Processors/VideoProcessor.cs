using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    internal class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
            MetadataFactory = dependencyProvider.VideoMetadataFactory;
        }

        private IFFMPEGRunner FFPMPEGRunner => DependencyProvider.VideoFFMPEGRunner;
        private IFFProbeRunner FFProbeRunner => DependencyProvider.FFProbeRunner;

        private IEmbeddedMetadataFactory<VideoPodMetadata> MetadataFactory { get; }

        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<VideoPodMetadata>(filesToProcess);

            await NormalizeOriginals(filesToProcess, metadata);

            // verify normalized versions of originals
            await FFPMPEGRunner.Verify(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<ObjectFileModel>().Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();

            processedList = processedList.Concat(await CreateMezzanineDerivatives(processedList, metadata)).ToList();

            // now remove duplicate entries -- this could happen if mezzanine master
            // already exists
            processedList = processedList.RemoveDuplicates();

            // now create access models
            processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();

            // create QC files
            var qcFiles = await CreateQualityControlFiles(processedList);

            // concat processed list and output list and return
            return new List<AbstractFileModel>().Concat(processedList).Concat(qcFiles).ToList();
        }

        private async Task NormalizeOriginals(List<ObjectFileModel> originals, VideoPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = MetadataFactory.Generate(originals, original, podMetadata);
                await FFPMPEGRunner.Normalize(original, metadata);
            }
        }

        private async Task<List<ObjectFileModel>> CreateMezzanineDerivatives(List<ObjectFileModel> models, VideoPodMetadata metadata)
        {
            var results = new List<ObjectFileModel>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = master.ToMezzanineFileModel();
                var embeddedMetadata = MetadataFactory.Generate(models, derivative, metadata);
                results.Add(await FFPMPEGRunner.CreateProdOrMezzDerivative(master, derivative, embeddedMetadata));
            }

            return results;
        }

        private async Task<List<ObjectFileModel>> CreateAccessDerivatives(IEnumerable<ObjectFileModel> models)
        {
            var results = new List<ObjectFileModel>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsMezzanineVersion()))
            {
                results.Add(await FFPMPEGRunner.CreateAccessDerivative(model));
            }
            return results;
        }

        private async Task<List<QualityControlFileModel>> CreateQualityControlFiles(IEnumerable<ObjectFileModel> processedList)
        {
            var results = new List<QualityControlFileModel>();
            foreach (var model in processedList.Where(m=> m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model));
            }

            return results;
        }
    }
}