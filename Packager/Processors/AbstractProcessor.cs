using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;

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

        private IProgramSettings ProgramSettings
        {
            get { return _dependencyProvider.ProgramSettings; }
        }

        protected IObserverCollection Observers
        {
            get { return _dependencyProvider.Observers; }
        }

        protected abstract string ProductionFileExtension { get; }
        protected abstract string AccessFileExtension { get; }
        protected abstract string MezzanineFileExtension { get; }
        protected abstract string PreservationFileExtension { get; }
        protected abstract string PreservationIntermediateFileExtenstion { get; }

        protected IPodMetadataProvider MetadataProvider
        {
            get { return _dependencyProvider.MetadataProvider; }
        }

        protected IXmlExporter XmlExporter
        {
            get { return _dependencyProvider.XmlExporter; }
        }

        protected string ProjectCode
        {
            get { return ProgramSettings.ProjectCode; }
        }

        private string ObjectDirectoryName
        {
            get { return string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode); }
        }

        protected string ProcessingDirectory
        {
            get { return Path.Combine(RootProcessingDirectory, ObjectDirectoryName); }
        }

        private string DropBoxDirectory
        {
            get { return Path.Combine(RootDropBoxDirectory, ObjectDirectoryName); }
        }

        protected string Barcode { get; private set; }

        public virtual async Task<bool> ProcessFile(IGrouping<string, AbstractFileModel> fileModels)
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

                Observers.EndSection(sectionKey, string.Format("Object processed succesfully: {0}", Barcode), true);
                return true;
            }
            catch (Exception e)
            {
                Observers.LogIssue(e);
                MoveToErrorFolder();
                Observers.EndSection(sectionKey);
                return false;
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

        protected abstract Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel);
        protected abstract Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess);
        
        // ReSharper disable once InconsistentNaming
        protected string BWFMetaEditPath
        {
            get { return ProgramSettings.BWFMetaEditPath; }
        }

        protected string DateFormat
        {
            get { return ProgramSettings.DateFormat; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGPath
        {
            get { return ProgramSettings.FFMPEGPath; }
        }

        private string InputDirectory
        {
            get { return ProgramSettings.InputDirectory; }
        }

        private string RootDropBoxDirectory
        {
            get { return ProgramSettings.DropBoxDirectoryName; }
        }

        private string RootProcessingDirectory
        {
            get { return ProgramSettings.ProcessingDirectory; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioProductionArguments
        {
            get { return ProgramSettings.FFMPEGAudioProductionArguments; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioAccessArguments
        {
            get { return ProgramSettings.FFMPEGAudioAccessArguments; }
        }

        protected string DigitizingEntity
        {
            get { return ProgramSettings.DigitizingEntity; }
        }

        protected string SuccesDirectory
        {
            get { return Path.Combine(ProgramSettings.SuccessDirectoryName, ObjectDirectoryName); }
        }

        protected string ErrorDirectory
        {
            get { return Path.Combine(ProgramSettings.ErrorDirectoryName, ObjectDirectoryName); }
        }

        protected IFileProvider FileProvider
        {
            get { return _dependencyProvider.FileProvider; }
        }

        protected IDirectoryProvider DirectoryProvider
        {
            get { return _dependencyProvider.DirectoryProvider; }
        }

        protected IProcessRunner ProcessRunner
        {
            get { return _dependencyProvider.ProcessRunner; }
        }

        protected IMetadataGenerator MetadataGenerator
        {
            get { return _dependencyProvider.MetadataGenerator; }
        }

        protected IBextProcessor BextProcessor
        {
            get { return _dependencyProvider.BextProcessor; }
        }

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

                Observers.EndSection(sectionKey, "Initialization successful", true);
            }
            catch (Exception e)
            {
                Observers.LogIssue(e);
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

                Observers.EndSection(sectionKey, "Cleanup successful", true);
            }
            catch (Exception e)
            {
                Observers.LogIssue(e);
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
                Observers.EndSection(sectionKey, string.Format("{0} files copied to dropbox folder successfully", Barcode), true);
            }
            catch (Exception e)
            {
                Observers.LogIssue(e);
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

        protected async Task<ConsolidatedPodMetadata> GetMetadata()
        {
            var sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
            try
            {
                var metadata = await MetadataProvider.Get(Barcode);

                Observers.Log("{0}", metadata);
                Observers.EndSection(sectionKey, string.Format("Retrieved metadata for object: {0}", Barcode), true);
                return metadata;
            }
            catch (Exception e)
            {
                Observers.LogIssue(e);
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