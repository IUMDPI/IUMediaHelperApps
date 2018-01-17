using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Packager.Exceptions;
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
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;
using ValidationResult = Packager.Validators.ValidationResult;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor<AudioPodMetadata>
    {
        public AudioProcessor(
            IBextProcessor bextProcessor, 
            IDirectoryProvider directoryProvider, 
            IFileProvider fileProvider, 
            IHasher hasher, 
            IPodMetadataProvider metadataProvider, 
            IObserverCollection observers, 
            IProgramSettings programSettings, 
            IXmlExporter xmlExporter, 
            ICarrierDataFactory<AudioPodMetadata> carrierDataFactory, 
            IEmbeddedMetadataFactory<AudioPodMetadata> embeddedMetadataFactory, 
            IFFMPEGRunner ffMpegRunner, 
            ILabelImageImporter imageProcessor,
            IPlaceHolderFactory placeHolderFactory) : base(
                bextProcessor, 
                directoryProvider, 
                fileProvider, 
                hasher, 
                metadataProvider, 
                observers, 
                programSettings, 
                xmlExporter,
                imageProcessor,
                placeHolderFactory)
        {
            CarrierDataFactory = carrierDataFactory;
            EmbeddedMetadataFactory = embeddedMetadataFactory;
            FFMpegRunner = ffMpegRunner;
        }

        protected override ICarrierDataFactory<AudioPodMetadata> CarrierDataFactory { get; }
        protected override IFFMPEGRunner FFMpegRunner { get; }
        protected override IEmbeddedMetadataFactory<AudioPodMetadata> EmbeddedMetadataFactory { get; } 
           
        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        protected override AbstractFile CreateProdOrMezzModel(AbstractFile master)
        {
            return new ProductionFile(master);
        }

        protected override IEnumerable<AbstractFile> GetProdOrMezzModels(IEnumerable<AbstractFile> models)
        {
            return models.Where(m => m.IsProductionVersion());
        }

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList, CancellationToken cancellationToken)
        {
            var sectionKey = Observers.BeginSection("Clearing metadata fields");
            try
            {
                var fieldsToClear = new List<BextFields> {BextFields.ISFT, BextFields.ITCH};
                await BextProcessor.ClearMetadataFields(processedList, fieldsToClear, cancellationToken);
                Observers.EndSection(sectionKey, "Metadata fields cleared successfully");
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList, CancellationToken cancellationToken)
        {
            // do nothing here
            return await Task.FromResult(new List<AbstractFile>());
        }

        protected override ValidationResult ContinueProcessingObject(AbstractPodMetadata metadata)
        {
            var sectionKey = Observers.BeginSection("Checking prerequisites");
            try
            {
                if (metadata.Format != MediaFormats.LacquerDisc)
                {
                    Observers.Log("All prerequisites present");
                    return ValidationResult.Success;
                }

                if (metadata.Unit.Equals("Archives of Traditional Music") == false)
                {
                    Observers.Log("All prerequisites present");
                    return ValidationResult.Success;
                }

                var result =  LabelImageImporter.LabelImagesPresent(metadata)
                    ? ValidationResult.Success
                    : new ValidationResult("Label images are not preset.");

                Observers.Log(!result.Result ? "Label images not present" : "All prerequisites present");

                return result;
            }
            finally
            {
                Observers.EndSection(sectionKey);
            }
        }
    }
}