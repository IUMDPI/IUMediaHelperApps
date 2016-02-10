using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor<AudioPodMetadata>
    {
        public AudioProcessor(IDependencyProvider dependencyProvider)
            : base(dependencyProvider)
        {
            EmbeddedMetadataFactory = DependencyProvider.AudioMetadataFactory;
            CarrierDataFactory = DependencyProvider.AudioCarrierDataFactory;
            FFMpegRunner = DependencyProvider.AudioFFMPEGRunner;
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

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList)
        {
            var sectionKey = Observers.BeginSection("Clearing metadata fields");
            try
            {
                await
                    BextProcessor.ClearMetadataFields(processedList,
                        new List<BextFields> {BextFields.ISFT, BextFields.ITCH});
                Observers.EndSection(sectionKey, "Metadata fields cleared successfully");
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList)
        {
            // do nothing here
            return await Task.FromResult(new List<AbstractFile>());
        }
    }
}