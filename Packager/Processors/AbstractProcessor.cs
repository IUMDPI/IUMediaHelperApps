using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;
using Packager.Validators;

namespace Packager.Processors
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IDependencyProvider _dependencyProvider;
        // constructor
        protected AbstractProcessor(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        private IProgramSettings ProgramSettings => _dependencyProvider.ProgramSettings;

        protected IObserverCollection Observers => _dependencyProvider.Observers;

        protected abstract string ProductionFileExtension { get; }
        protected abstract string AccessFileExtension { get; }
        protected abstract string MezzanineFileExtension { get; }
        protected abstract string PreservationFileExtension { get; }
        protected abstract string PreservationIntermediateFileExtenstion { get; }

        private IPodMetadataProvider MetadataProvider => _dependencyProvider.MetadataProvider;

        protected IXmlExporter XmlExporter => _dependencyProvider.XmlExporter;

        protected string ProjectCode => ProgramSettings.ProjectCode;

        private string ObjectDirectoryName => $"{ProjectCode.ToUpperInvariant()}_{Barcode}";

        protected string ProcessingDirectory => Path.Combine(RootProcessingDirectory, ObjectDirectoryName);

        private string DropBoxDirectory => Path.Combine(RootDropBoxDirectory, ObjectDirectoryName);

        protected string Barcode { get; private set; }

        public virtual async Task<ValidationResult> ProcessFile(IGrouping<string, AbstractFileModel> fileModels)
        {
            Barcode = fileModels.Key;

            var sectionKey = Observers.BeginSection("Processing Object: {0}", Barcode);
            try
            {
                // figure out what files we need to touch
                var filesToProcess = GetFilesToProcess(fileModels);

                // now move them to processing
                await CreateProcessingDirectoryAndMoveFiles(filesToProcess);

                // call internal implementation
                var processedList = await ProcessFileInternal(filesToProcess);

                // copy processed files to drop box
                await CopyToDropbox(processedList);

                // move everything to success folder
                await MoveToSuccessFolder();

                Observers.EndSection(sectionKey, $"Object processed succesfully: {Barcode}");
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                MoveToErrorFolder();
                Observers.EndSection(sectionKey);
                return new ValidationResult(e.Message);
            }
        }

        private static List<ObjectFileModel> GetFilesToProcess(IEnumerable<AbstractFileModel> fileModels)
        {
            return fileModels
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel)m)
                .Where(m => m.IsPreservationIntermediateVersion() || m.IsPreservationVersion() || m.IsProductionVersion())
                .ToList();
        }

        protected abstract Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess);
        
        private string InputDirectory => ProgramSettings.InputDirectory;

        private string RootDropBoxDirectory => ProgramSettings.DropBoxDirectoryName;

        private string RootProcessingDirectory => ProgramSettings.ProcessingDirectory;

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioProductionArguments => ProgramSettings.FFMPEGAudioProductionArguments;

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioAccessArguments => ProgramSettings.FFMPEGAudioAccessArguments;

        private string SuccesDirectory => Path.Combine(ProgramSettings.SuccessDirectoryName, ObjectDirectoryName);

        private string ErrorDirectory => Path.Combine(ProgramSettings.ErrorDirectoryName, ObjectDirectoryName);

        private IFileProvider FileProvider => _dependencyProvider.FileProvider;

        private IDirectoryProvider DirectoryProvider => _dependencyProvider.DirectoryProvider;

        protected ICarrierDataFactory MetadataGenerator => _dependencyProvider.MetadataGenerator;

        protected IBextProcessor BextProcessor => _dependencyProvider.BextProcessor;

        // ReSharper disable once InconsistentNaming
        protected IFFMPEGRunner IffmpegRunner => _dependencyProvider.FFMPEGRunner;

        private async Task CreateProcessingDirectoryAndMoveFiles(IEnumerable<ObjectFileModel> filesToProcess)
        {
            var sectionKey = Observers.BeginSection("Initializing");
            try
            {
                // make directory to hold processed files
                Observers.Log("Creating directory: {0}", ProcessingDirectory);
                DirectoryProvider.CreateDirectory(ProcessingDirectory);
                
                foreach (var fileModel in filesToProcess)
                {
                    Observers.Log("Moving file to processing: {0}", fileModel.OriginalFileName);
                    await MoveFileToProcessing(fileModel);
                }

                Observers.EndSection(sectionKey, "Initialization successful");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task MoveToSuccessFolder()
        {
            var sectionKey = Observers.BeginSection("Cleaning up");
            try
            {
                Observers.Log("Moving {0} to success directory", ObjectDirectoryName);
                await DirectoryProvider.MoveDirectoryAsync(ProcessingDirectory, SuccesDirectory);

                Observers.EndSection(sectionKey, "Cleanup successful");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task CopyToDropbox(IEnumerable<AbstractFileModel> fileList)
        {
            var sectionKey = Observers.BeginSection("Copying objects to dropbox");
            try
            {
                DirectoryProvider.CreateDirectory(DropBoxDirectory);

                foreach (var fileName in fileList.Select(fileModel => fileModel.ToFileName()).OrderBy(f => f))
                {
                    Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                    await FileProvider.CopyFileAsync(
                        Path.Combine(ProcessingDirectory, fileName),
                        Path.Combine(DropBoxDirectory, fileName));
                }
                Observers.EndSection(sectionKey, $"{Barcode} files copied to dropbox folder successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private void MoveToErrorFolder()
        {
            try
            {
                // move folder to success
                Observers.Log("Moving {0} to error directory", ObjectDirectoryName);

                DirectoryProvider.MoveDirectory(ProcessingDirectory, ErrorDirectory);
            }
            catch (Exception e)
            {
                Observers.Log("Could not move {0} to error directory: {1}", ObjectDirectoryName, e);
            }
        }

        private async Task<string> MoveFileToProcessing(AbstractFileModel fileModel)
        {
            var sourcePath = Path.Combine(InputDirectory, fileModel.OriginalFileName);
            var targetPath = Path.Combine(ProcessingDirectory, fileModel.ToFileName()); // ToFileName will normalize the filename when we move the file
            await FileProvider.MoveFileAsync(sourcePath, targetPath);
            return targetPath;
        }

        protected async Task<ConsolidatedPodMetadata> GetMetadata(List<ObjectFileModel> filesToProcess)
        {
            var sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
            try
            {
                var metadata = await MetadataProvider.Get(Barcode);

                // log metadata
                MetadataProvider.Log(metadata);

                // validate base metadata fields
                MetadataProvider.Validate(metadata, filesToProcess);

                Observers.EndSection(sectionKey, $"Retrieved metadata for object: {Barcode}");
                
                return metadata;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        protected ObjectFileModel ToAccessFileModel(ObjectFileModel original)
        {
            return original.ToAccessFileModel(AccessFileExtension);
        }

        protected ObjectFileModel ToMezzanineFileModel(ObjectFileModel original)
        {
            return original.ToMezzanineFileModel(MezzanineFileExtension);
        }

        protected ObjectFileModel ToProductionFileModel(ObjectFileModel original)
        {
            return original.ToProductionFileModel(ProductionFileExtension);
        }
    }
}