using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Packager.Extensions;
using Packager.Factories;
using Packager.Factories.FFMPEGArguments;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Images;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;
using Packager.Validators;

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
            IFFMPEGRunner ffmpegRunner, 
            IFFMPEGArgumentsFactory ffmpegArgumentsFactory,
            IFFProbeRunner ffProbeRunner, 
            IMediaInfoProvider mediaInfoProvider,
            ILabelImageImporter imageProcessor, 
            IPlaceHolderFactory placeHolderFactory) : base(
                bextProcessor, 
                directoryProvider, 
                fileProvider, 
                hasher, 
                metadataProvider, 
                ffmpegRunner,
                ffmpegArgumentsFactory,
                observers, 
                programSettings, 
                xmlExporter, 
                imageProcessor, 
                placeHolderFactory)
        {
            CarrierDataFactory = carrierDataFactory;
            EmbeddedMetadataFactory = embeddedMetadataFactory;
            FFProbeRunner = ffProbeRunner;
            MediaInfoProvider = mediaInfoProvider;
        }

        private const string ReduceStreamsAccessArguments =
            "-filter_complex \"[0:a:0][0:a:1]amerge=inputs=2[aout]\" -map 0:v -map \"[aout]\"";

        private IFFProbeRunner FFProbeRunner { get; }
        private IMediaInfoProvider MediaInfoProvider { get; }
        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");
        protected override ICarrierDataFactory<VideoPodMetadata> CarrierDataFactory { get; }
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

        protected override async Task<AbstractFile> CreateAccessDerivative(AbstractFile model, IMediaFormat format, CancellationToken cancellationToken)
        {
            var notes = new List<string>();

            var arguments = FFMPEGArgumentsFactory.GetAccessArguments(format);

            var mediaInfo = await MediaInfoProvider.GetMediaInfo(model, cancellationToken);
            if (mediaInfo.AudioStreams > 1)
            {
                notes.Add("Multiple audio streams present; merging audio streams.");
                arguments.AddArguments(ReduceStreamsAccessArguments);
            }

            return await FFMpegRunner.CreateAccessDerivative(model, arguments, notes, cancellationToken);
        }

        protected override ValidationResult ContinueProcessingObject(AbstractPodMetadata metadata)
        {
            return ValidationResult.Success;
        }
    }
}