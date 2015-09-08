using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor
    {
        // todo: figure out how to get this
        //private const string TempInstitution = "Indiana University, Bloomington. William and Gayle Cook Music Library";

        public AudioProcessor(IDependencyProvider dependencyProvider)
            : base(dependencyProvider)
        {
        }

        protected override string ProductionFileExtension => ".wav";
        protected override string AccessFileExtension => ".mp4";
        protected override string MezzanineFileExtension => ".aac";
        protected override string PreservationFileExtension => ".wav";
        protected override string PreservationIntermediateFileExtenstion => ".wav";

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata(filesToProcess);

            // create derivatives for the various files
            // and add them to a list of files that have
            // been processed
            var processedList = new List<AbstractFileModel>();

            // add two lists together, but remove duplicates
            // duplicate file entries might happen if a .prod version already exists
            // because it was create by an audio engineer
            processedList = processedList.Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();

            // first group by sequence
            // then determine which file to use to create derivatives
            // then use that file to create the derivatives
            // then aggregate the results into the processed list
            foreach (var model in filesToProcess
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivatives = await CreateDerivatives(model);
                processedList = processedList.Concat(derivatives).ToList();
            }

            // now remove duplicate entries -- this could happen if production master
            // already exists
            processedList = processedList
                .GroupBy(o => o.ToFileName())
                .Select(g => g.First()).ToList();


            // now remove metadata fields that we don't want
            await ClearMetadata(processedList);

            // now add metadata to eligible objects
            await AddMetadata(processedList, metadata);
            
            // using the list of files that have been processed
            // make the xml file
            var xmlModel = await GenerateXml(metadata, processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel) m).ToList());

            processedList.Add(xmlModel);

            return processedList;
        }

        protected virtual async Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel)
        {
            var prodModel = await IffmpegRunner.CreateDerivative(
                fileModel,
                ToProductionFileModel(fileModel),
                FFMPEGAudioProductionArguments);

            var accessModel = await IffmpegRunner.CreateDerivative(
                prodModel,
                ToAccessFileModel(prodModel),
                FFMPEGAudioAccessArguments);

            // return models for files
            return new List<ObjectFileModel> {prodModel, accessModel};
        }

        private async Task AddMetadata(IEnumerable<AbstractFileModel> processedList, ConsolidatedPodMetadata podMetadata)
        {
            var sectionKey = string.Empty;
            var success = false;
            try
            {
                sectionKey = Observers.BeginSection("Adding BEXT metadata");
                var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                    .Select(m => (ObjectFileModel) m)
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
                    .Select(m => (ObjectFileModel) m)
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
                
                var wrapper = new IU {Carrier = MetadataGenerator.Generate(metadata, filesToProcess)};
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.ToFileName()), new UTF8Encoding(false));

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