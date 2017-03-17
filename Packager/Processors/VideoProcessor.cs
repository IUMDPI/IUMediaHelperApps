using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Images;
using Packager.Utilities.PlaceHolderGenerators;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;

namespace Packager.Processors
{
    public class VideoProcessor : AbstractProcessor<VideoPodMetadata>
    {
        public VideoProcessor(
            IBextProcessor bextProcessor, 
            IDirectoryProvider directoryProvider, 
            IFileProvider fileProvider, 
            IHasher hasher, 
            IPodMetadataProvider metadataProvider, 
            IObserverCollection observers, 
            IProgramSettings programSettings, 
            IXmlExporter xmlExporter, 
            ICarrierDataFactory<VideoPodMetadata> carrierDataFactory, 
            IEmbeddedMetadataFactory<VideoPodMetadata> embeddedMetadataFactory, 
            IFFMPEGRunner ffMpegRunner, 
            IFFProbeRunner ffProbeRunner, 
            ILabelImageImporter imageProcessor, 
            IPlaceHolderGenerator placeHolderGenerator) : base(
                bextProcessor, 
                directoryProvider, 
                fileProvider, 
                hasher, 
                metadataProvider, 
                observers, 
                programSettings, 
                xmlExporter, 
                imageProcessor, 
                placeHolderGenerator)
        {
            CarrierDataFactory = carrierDataFactory;
            EmbeddedMetadataFactory = embeddedMetadataFactory;
            FFMpegRunner = ffMpegRunner;
            FFProbeRunner = ffProbeRunner;
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

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList, CancellationToken cancellationToken)
        {
            // do nothing
            await Task.FromResult(0);
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList, CancellationToken cancellationToken)
        {
            var results = new List<AbstractFile>();
            foreach (
                var model in
                    processedList.Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model, cancellationToken));
            }

            return results;
        }
    }
}