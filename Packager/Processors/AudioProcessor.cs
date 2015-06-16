using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.BextModels;
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
        private const string TempInstitution = "Indiana University, Bloomington. William and Gayle Cook Music Library";

        public AudioProcessor(string barcode, IDependencyProvider dependencyProvider)
            : base(barcode, dependencyProvider)
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

        protected override async Task ProcessFileInternal(IEnumerable<AbstractFileModel> fileModels)
        {
            Observers.LogHeader("Processing object {0}", Barcode);

            // make directory to hold processed files
            DirectoryProvider.CreateDirectory(Path.Combine(ProcessingDirectory));

            var filesToProcess = fileModels
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsPreservationIntermediateVersion() || m.IsPreservationVersion())
                .ToList();

            // now move them to processing
            foreach (var fileModel in filesToProcess)
            {
                Observers.Log("Moving file to processing: {0}", fileModel.OriginalFileName);
                await MoveFileToProcessing(fileModel.ToFileName());
            }

            // fetch metadata
            var metadata = await GetMetadata();

            // create derivatives for the various files
            // and add them to a list of files that have
            // been processed
            var processedList = new List<AbstractFileModel>();
            processedList = processedList.Concat(filesToProcess).ToList();

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

            // now add metadata to eligible objects
            await AddMetadata(processedList, metadata);

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = GenerateXml(metadata,
                processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel) m).ToList());

            processedList.Add(xmlModel);

            // make directory to hold completed files
            DirectoryProvider.CreateDirectory(DropBoxDirectory);

            // copy files
            foreach (var fileName in processedList.Select(fileModel => fileModel.ToFileName()).OrderBy(f => f))
            {
                Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                await FileProvider.CopyFileAsync(
                    Path.Combine(ProcessingDirectory, fileName),
                    Path.Combine(DropBoxDirectory, fileName));
            }
        }

        public override async Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel)
        {
            var fileName = fileModel.ToFileName();

            Observers.LogHeader("Generating Production Version: {0}", fileName);
            var prodModel = await CreateDerivative(
                fileModel,
                ToProductionFileModel(fileModel),
                AddNoOverwriteToFfmpegCommand(FFMPEGAudioProductionArguments));

            Observers.LogHeader("Generating AccessVersion: {0}", fileName);
            var accessModel = await CreateDerivative(
                prodModel,
                ToAccessFileModel(prodModel),
                AddNoOverwriteToFfmpegCommand(FFMPEGAudioAccessArguments));

            // return models for files
            return new List<ObjectFileModel> {prodModel, accessModel};
        }

        private static string AddNoOverwriteToFfmpegCommand(string arguments)
        {
            if (arguments.ToLowerInvariant().Contains("-y") || arguments.ToLowerInvariant().Contains("-n"))
            {
                return arguments;
            }

            return arguments + " -n";
        }

        private async Task<ObjectFileModel> CreateDerivative(AbstractFileModel originalModel, ObjectFileModel newModel, string commandLineArgs)
        {
            var inputPath = Path.Combine(ProcessingDirectory, originalModel.ToFileName());
            var outputPath = Path.Combine(ProcessingDirectory, newModel.ToFileName());

            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            await CreateDerivative(args);
            return newModel;
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

            Observers.LogExternal(result.StandardError);

            var verifier = new FFMPEGVerifier(result.ExitCode);
            if (!verifier.Verify())
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }

        private async Task AddMetadata(IEnumerable<AbstractFileModel> processedList, ConsolidatedPodMetadata podMetadata)
        {
            Observers.Log("\nAdding metadata to objects");
            var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsAccessVersion() == false).ToList();

            if (!filesToAddMetadata.Any())
            {
                throw new AddMetadataException("Could not add metadata: no eligible files");
            }

            var xml = new ConformancePointDocumentFactory(FileProvider, ProcessingDirectory, DigitizingEntity, TempInstitution)
                .Get(filesToAddMetadata, podMetadata);

            await AddMetadata(xml);
        }

        private async Task AddMetadata(ConformancePointDocument xml)
        {
            var xmlPath = Path.Combine(ProcessingDirectory, "core.xml");
            XmlExporter.ExportToFile(xml, xmlPath);

            var args = string.Format("--verbose --Append --in-core={0}", xmlPath.ToQuoted());

            var startInfo = new ProcessStartInfo(BWFMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run(startInfo);

            Observers.LogExternal(result.StandardOutput);

            var verifier = new BwfMetaEditResultsVerifier(
                result.StandardOutput.ToLowerInvariant(),
                xml.File.Select(f => f.Name.ToLowerInvariant()).ToList(),
                Observers);

            if (!verifier.Verify())
            {
                throw new AddMetadataException("Could not add metadata to one or more files!");
            }
        }

        private XmlFileModel GenerateXml(ConsolidatedPodMetadata metadata, IEnumerable<ObjectFileModel> filesToProcess)
        {
            var result = new XmlFileModel { BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml" };
            var wrapper = new IU {Carrier = MetadataGenerator.GenerateMetadata(metadata, filesToProcess, ProcessingDirectory)};
            XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.ToFileName()));
            
            return result;
        }

        
    }
}