using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Verifiers;

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

        protected override string ProductionFileExtension
        {
            get { return ".wav"; }
        }

        protected override string AccessFileExtension
        {
            get { return ".mp4"; }
        }

        protected override string MezzanineFileExtension
        {
            get { return ".aac"; }
        }

        protected override string PreservationFileExtension
        {
            get { return ".wav"; }
        }

        protected override string PreservationIntermediateFileExtenstion
        {
            get { return ".wav"; }
        }

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

            // now add metadata to eligible objects
            await AddMetadata(processedList, metadata);

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = GenerateXml(metadata,
                processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel) m).ToList());

            processedList.Add(xmlModel);

            return processedList;
        }

        protected override async Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel)
        {
            var prodModel = await CreateDerivative(
                fileModel,
                ToProductionFileModel(fileModel),
                FFMPEGAudioProductionArguments);

            var accessModel = await CreateDerivative(
                prodModel,
                ToAccessFileModel(prodModel),
                FFMPEGAudioAccessArguments);

            // return models for files
            return new List<ObjectFileModel> {prodModel, accessModel};
        }

        private async Task<ObjectFileModel> CreateDerivative(AbstractFileModel originalModel, ObjectFileModel newModel, string commandLineArgs)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", newModel.FullFileUse, newModel.ToFileName());
            try
            {
                var inputPath = Path.Combine(ProcessingDirectory, originalModel.ToFileName());
                var outputPath = Path.Combine(ProcessingDirectory, newModel.ToFileName());

                if (FileProvider.FileExists(outputPath))
                {
                    Observers.Log("{0} already exists. Will not generate derivate", newModel.FullFileUse);
                }
                else
                {
                    var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);
                    await CreateDerivative(args);
                }

                Observers.EndSection(sectionKey, string.Format("{0} generated successfully: {1}", newModel.FullFileUse, newModel.ToFileName()));
                return newModel;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task CreateDerivative(string args)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run(startInfo);

            Observers.Log(result.StandardError);

            var verifier = new FFMPEGVerifier();
            if (!verifier.Verify(result.ExitCode))
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }

        private async Task AddMetadata(IEnumerable<AbstractFileModel> processedList, ConsolidatedPodMetadata podMetadata)
        {
            var sectionKey = Guid.Empty;
            var success = false;
            try
            {
                sectionKey = Observers.BeginSection("Adding BEXT metadata");
                var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                    .Select(m => (ObjectFileModel) m)
                    .Where(m => m.IsAccessVersion() == false).ToList();

                if (!filesToAddMetadata.Any())
                {
                    throw new AddMetadataException("Could not add metadata: no eligible files");
                }

                await BextProcessor.EmbedBextMetadata(filesToAddMetadata, podMetadata, ProcessingDirectory);

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

        private XmlFileModel GenerateXml(ConsolidatedPodMetadata metadata, IEnumerable<ObjectFileModel> filesToProcess)
        {
            var result = new XmlFileModel {BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml"};
            var wrapper = new IU {Carrier = MetadataGenerator.Generate(metadata, filesToProcess, ProcessingDirectory)};
            XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.ToFileName()));

            return result;
        }
    }
}