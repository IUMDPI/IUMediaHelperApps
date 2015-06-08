using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.ProcessResults;
using Packager.Providers;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor
    {
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

        public override async Task ProcessFile(IGrouping<string, AbstractFileModel> barcodeGrouping)
        {
            Barcode = barcodeGrouping.Key;
            Observers.LogHeader("Processing object {0}", Barcode);

            // fetch metadata
            var metadata = await GetMetadata();
            
            // make directory to hold processed files
            DirectoryProvider.CreateDirectory(Path.Combine(ProcessingDirectory));

         
            
            var filesToProcess = barcodeGrouping
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsPreservationIntermediateVersion() || m.IsPreservationVersion())
                .ToList();

            // now move them to processing
            foreach (var fileModel in filesToProcess)
            {
                Observers.Log("Moving file to processing: {0}", fileModel.OriginalFileName);
                MoveFileToProcessing(fileModel.ToFileName());
            }

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
            await AddMetadata(processedList);

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = GenerateXml(metadata,
                processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel) m).ToList());

            processedList.Add(xmlModel);

            // make directory to hold completed files
            DirectoryProvider.CreateDirectory(DropBoxDirectory);

            // copy files
            // todo: make asyc

            foreach (var fileName in processedList.Select(fileModel => fileModel.ToFileName()).OrderBy(f => f))
            {
                Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                FileProvider.Copy(
                    Path.Combine(ProcessingDirectory, fileName),
                    Path.Combine(DropBoxDirectory, fileName));
            }

            // done - log new line
            Observers.Log("");
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

            var result = await ProcessRunner.Run<FFMPEGProcessResult>(startInfo);

            Observers.Log(result.StandardError);

            if (!result.Succeeded())
            {
                throw new Exception(string.Format("Could not generate derivative: {0}", result.ExitCode));
            }
        }

        private async Task AddMetadata(IEnumerable<AbstractFileModel> processedList)
        {
            var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsAccessVersion() == false).ToList();

            if (!filesToAddMetadata.Any())
            {
                throw new Exception("Could not add metadata: no eligible files");
            }

            var data = BextDataProvider.GetMetadata(Barcode);

            foreach (var fileModel in filesToAddMetadata)
            {
                await AddMetadata(fileModel, data);
            }
        }

        private async Task AddMetadata(AbstractFileModel fileModel, BextData data)
        {
            var targetPath = Path.Combine(ProcessingDirectory, fileModel.ToFileName());
            Observers.LogHeader("Embedding Metadata: {0}", fileModel.ToFileName());

            await AddMetadata(targetPath, data);
        }

        private async Task AddMetadata(string targetPath, BextData data)
        {
            var args = string.Format("--verbose --Append {0} {1}", string.Join(" ", data.GenerateCommandArgs()), targetPath);

            var startInfo = new ProcessStartInfo(BWFMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run<BwfMetaEditProcessResult>(startInfo);

            Observers.Log(result.StandardOutput);

            if (!result.Succeeded())
            {
                throw new Exception(string.Format("Could not insert BEXT Data: {0}", result.ExitCode));
            }
        }

        private XmlFileModel GenerateXml(PodMetadata metadata, List<ObjectFileModel> filesToProcess)
        {
            var wrapper = new IU {Carrier = MetadataGenerator.GenerateMetadata(metadata, filesToProcess, ProcessingDirectory)};
            var xml = XmlExporter.GenerateXml(wrapper);

            var result = new XmlFileModel {BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml"};
            SaveXmlFile(string.Format(result.ToFileName(), ProjectCode, Barcode), xml);
            return result;
        }

        private void SaveXmlFile(string filename, string xml)
        {
            FileProvider.WriteAllText(Path.Combine(ProcessingDirectory, filename), xml);
        }
    }
}