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

        private IBextMetadataFactory AudioMetadataFactory { get; }

        protected override string ProductionFileExtension => ".wav";
        protected override string AccessFileExtension => ".mp4";
        protected override string MezzanineFileExtension => ".aac";
        protected override string PreservationFileExtension => ".wav";
        protected override string PreservationIntermediateFileExtenstion => ".wav";

        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata(filesToProcess);

            // normalize originals
            await NormalizeOriginals(filesToProcess, metadata);

            // verify normalized versions of originals
            await FFPMpegRunner.Verify(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<ObjectFileModel>().Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();

            // next group by sequence
            // then determine which file to use to create derivatives
            // then use that file to create the derivatives
            // then aggregate the results into the processed list
            processedList = processedList.Concat(await CreateProductionDerivatives(processedList, metadata)).ToList();
            
            /*foreach (var model in filesToProcess
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                processedList.Add(await CreateProductionDerivative(model, metadata));
            }*/

            // now remove duplicate entries -- this could happen if production master
            // already exists
            processedList = processedList
                .GroupBy(o => o.ToFileName())
                .Select(g => g.First()).ToList();

            // now add metadata to eligible objects
            // await AddMetadata(processedList, metadata);

            // finally generate the access versions from production masters
            processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = await GenerateXml(metadata, processedList);

            var outputList = new List<AbstractFileModel>().Concat(processedList).ToList();
            outputList.Add(xmlModel);

            return outputList;
        }

        private async Task NormalizeOriginals(List<ObjectFileModel> originals, ConsolidatedPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = AudioMetadataFactory.Generate(originals, original, podMetadata);
                await FFPMpegRunner.Normalize(original, metadata);
            }
        }

        private async Task<List<ObjectFileModel>> CreateProductionDerivatives(List<ObjectFileModel> models, ConsolidatedPodMetadata podMetadata)
        {
            var results = new List<ObjectFileModel>();
            foreach (var model in models
               .GroupBy(m => m.SequenceIndicator)
               .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var metadata = AudioMetadataFactory.Generate(models, model, podMetadata);
                results.Add(await FFPMpegRunner.CreateProductionDerivative(model, metadata));
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

        

        private async Task AddMetadata(IEnumerable<AbstractFileModel> processedList, ConsolidatedPodMetadata podMetadata)
        {
            var sectionKey = string.Empty;
            var success = false;
            try
            {
                sectionKey = Observers.BeginSection("Adding BEXT metadata");
                var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                    .Select(m => (ObjectFileModel)m)
                    .Where(m => m.IsAccessVersion() == false).ToList();

                if (!filesToAddMetadata.Any())
                {
                    throw new BextMetadataException("Could not add metadata: no eligible files");
                }

                await BextProcessor.EmbedBextMetadata(filesToAddMetadata, podMetadata);

                success = true;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
            finally
            {
                if (success)
                {
                    Observers.EndSection(sectionKey, "BEXT metadata added successfully");
                }
                else
                {
                    Observers.EndSection(sectionKey);
                }
            }
        }

        private async Task ClearMetadata(IEnumerable<AbstractFileModel> processedList)
        {
            var sectionKey = string.Empty;
            var success = false;
            try
            {
                sectionKey = Observers.BeginSection("Clearing original BEXT metadata fields");
                var targets = processedList.Where(m => m.IsObjectModel())
                    .Select(m => (ObjectFileModel)m)
                    .Where(m => m.IsAccessVersion() == false).ToList();

                await BextProcessor.ClearAllBextMetadataFields(targets);

                success = true;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
            finally
            {
                if (success)
                {
                    Observers.EndSection(sectionKey, "Original BEXT metadata fields cleared successfully");
                }
                else
                {
                    Observers.EndSection(sectionKey);
                }
            }
        }

        private async Task<XmlFileModel> GenerateXml(ConsolidatedPodMetadata metadata, List<ObjectFileModel> filesToProcess)
        {
            var sectionKey = string.Empty;
            var success = false;
            var result = new XmlFileModel { BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml" };
            try
            {
                sectionKey = Observers.BeginSection("Generating {0}", result.ToFileName());

                await AssignChecksumValues(filesToProcess);

                var wrapper = new IU { Carrier = MetadataGenerator.Generate(metadata, filesToProcess) };
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.ToFileName()));

                success = true;
                return result;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
            finally
            {
                if (success)
                {
                    Observers.EndSection(sectionKey, $"{result.ToFileName()} generated successfully");
                }
                else
                {
                    Observers.EndSection(sectionKey);
                }
            }
        }
    }
}