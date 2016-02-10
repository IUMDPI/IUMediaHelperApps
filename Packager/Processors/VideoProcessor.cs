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
    public class VideoProcessor : AbstractProcessor<VideoPodMetadata>
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
            EmbeddedMetadataFactory = DependencyProvider.VideoMetadataFactory;
            FFMpegRunner = DependencyProvider.VideoFFMPEGRunner;
            FFProbeRunner = DependencyProvider.FFProbeRunner;
            CarrierDataFactory = DependencyProvider.VideoCarrierDataFactory;
        }
        
        private IFFProbeRunner FFProbeRunner { get; }
        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");
        protected override ICarrierDataFactory<VideoPodMetadata> CarrierDataFactory { get; }
        protected override IFFMPEGRunner FFMpegRunner { get; }
        protected override IEmbeddedMetadataFactory<VideoPodMetadata> EmbeddedMetadataFactory { get; }

        protected override AbstractFile CreateProdOrMezzModel(AbstractFile master)
        {
            return new MezzanineFile(master);
        }

        protected override IEnumerable<AbstractFile> GetProdOrMezzModels(IEnumerable<AbstractFile> models)
        {
            return models.Where(m => m.IsMezzanineVersion());
        }

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList)
        {
            // do nothing
            await Task.FromResult(0);
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList)
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