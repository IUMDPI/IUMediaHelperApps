using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    public class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
        }

        private IFFMPEGRunner FFPMPEGRunner => DependencyProvider.VideoFFMPEGRunner;
        private IFFProbeRunner FFProbeRunner => DependencyProvider.FFProbeRunner;
        private IEmbeddedMetadataFactory<VideoPodMetadata> MetadataFactory => DependencyProvider.VideoMetadataFactory;
        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");
        protected override ICarrierDataFactory CarrierDataFactory => DependencyProvider.VideoCarrierDataFactory;
        protected override IFFMPEGRunner FFMpegRunner => DependencyProvider.VideoFFMPEGRunner;

        protected override async Task<AbstractPodMetadata> GetMetadata(List<AbstractFile> filesToProcess)
        {
            return await GetMetadata<VideoPodMetadata>(filesToProcess);
        }

        protected override async Task NormalizeOriginals(List<AbstractFile> originals, AbstractPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = MetadataFactory.Generate(originals, original, (VideoPodMetadata) podMetadata);
                await FFPMPEGRunner.Normalize(original, metadata);
            }
        }

        protected override async Task<List<AbstractFile>> CreateProdOrMezzDerivatives(List<AbstractFile> models,
            AbstractPodMetadata metadata)
        {
            var results = new List<AbstractFile>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = new MezzanineFile(master);
                var embeddedMetadata = MetadataFactory.Generate(models, derivative, (VideoPodMetadata) metadata);
                results.Add(await FFPMPEGRunner.CreateProdOrMezzDerivative(master, derivative, embeddedMetadata));
            }

            return results;
        }

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList)
        {
            // do nothing
            await Task.FromResult(0);
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(List<AbstractFile> processedList)
        {
            var results = new List<AbstractFile>();
            foreach (
                var model in
                    processedList.Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model));
            }

            return results;
        }
    }
}