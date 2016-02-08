using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor
    {
        // todo: figure out how to get this
        //private const string TempInstitution = "Indiana University, Bloomington. William and Gayle Cook Music Library";

        public AudioProcessor(IDependencyProvider dependencyProvider)
            : base(dependencyProvider)
        {
            AudioMetadataFactory = dependencyProvider.AudioMetadataFactory;
        }

        // ReSharper disable once InconsistentNaming
        private IFFMPEGRunner FFPMpegRunner => DependencyProvider.AudioFFMPEGRunner;
        
        private IEmbeddedMetadataFactory<AudioPodMetadata> AudioMetadataFactory { get; }

        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        protected override async Task<IEnumerable<AbstractFile>> ProcessFileInternal(List<AbstractFile> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<AudioPodMetadata>(filesToProcess);
            
            // normalize originals
            await NormalizeOriginals(filesToProcess, metadata);

            // verify normalized versions of originals
            await FFPMpegRunner.Verify(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<AbstractFile>().Concat(filesToProcess)
                .GroupBy(m => m.Filename).Select(g => g.First()).ToList();

            // next group by sequence
            // then determine which file to use to create derivatives
            // then use that file to create the derivatives
            // then aggregate the results into the processed list
            processedList = processedList.Concat(await CreateProductionDerivatives(processedList, metadata)).ToList();

            // now remove duplicate entries -- this could happen if production master
            // already exists
            processedList = processedList.RemoveDuplicates();

            // now clear the ISFT field from presentation and production masters
            await ClearMetadataDataFields(processedList, new List<BextFields> {BextFields.ISFT, BextFields.ITCH});

            // finally generate the access versions from production masters
            processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();
            
            // using the list of files that have been processed
            // make the xml file
            var xmlModel = await GenerateXml(metadata, processedList);

            var outputList = new List<AbstractFile>().Concat(processedList).ToList();
            outputList.Add(xmlModel);

            return outputList;
        }
        
        private async Task NormalizeOriginals(List<AbstractFile> originals, AudioPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = AudioMetadataFactory.Generate(originals, original, podMetadata);
                await FFPMpegRunner.Normalize(original, metadata);
            }
        }

        private async Task<List<AbstractFile>> CreateProductionDerivatives(List<AbstractFile> models, AudioPodMetadata podMetadata)
        {
            var results = new List<AbstractFile>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = new ProductionFile(master);
                var metadata = AudioMetadataFactory.Generate(models, derivative, podMetadata);
                results.Add(await FFPMpegRunner.CreateProdOrMezzDerivative(master, derivative, metadata));
            }

            return results;
        }

        private async Task<List<AbstractFile>> CreateAccessDerivatives(IEnumerable<AbstractFile> models)
        {
            var results = new List<AbstractFile>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsProductionVersion()))
            {
                results.Add(await FFPMpegRunner.CreateAccessDerivative(model));
            }
            return results;
        }

        private async Task ClearMetadataDataFields(List<AbstractFile> instances, List<BextFields> fieldsToClear)
        {
            var sectionKey = Observers.BeginSection("Clearing metadata fields");
            try
            {
                await BextProcessor.ClearMetadataFields(instances, fieldsToClear);
                Observers.EndSection(sectionKey, "Metadata fields cleared successfully");
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
        }

        private async Task<XmlFile> GenerateXml(AudioPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            var result = new XmlFile(Barcode, ProjectCode);
            var sectionKey = Observers.BeginSection("Generating {0}", result.Filename);
            try
            {
                await AssignChecksumValues(filesToProcess);

                var wrapper = new IU {Carrier = MetadataGenerator.Generate(metadata, filesToProcess)};
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.Filename));

                Observers.EndSection(sectionKey, $"{result.Filename} generated successfully");
                return result;
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
        }

        protected async Task AssignChecksumValues(IEnumerable<AbstractFile> models)
        {
            foreach (var model in models)
            {
                model.Checksum = await Hasher.Hash(model);
                Observers.Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(model.Filename), model.Checksum);
            }
        }
    }
}