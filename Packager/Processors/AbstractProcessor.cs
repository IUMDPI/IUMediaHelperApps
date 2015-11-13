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

        private string InputDirectory => ProgramSettings.InputDirectory;

        private string RootDropBoxDirectory => ProgramSettings.DropBoxDirectoryName;

        private string RootProcessingDirectory => ProgramSettings.ProcessingDirectory;

        private IHasher Hasher => _dependencyProvider.Hasher;

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioProductionArguments => ProgramSettings.FFMPEGAudioProductionArguments;

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioAccessArguments => ProgramSettings.FFMPEGAudioAccessArguments;

        private IFileProvider FileProvider => _dependencyProvider.FileProvider;

        private IDirectoryProvider DirectoryProvider => _dependencyProvider.DirectoryProvider;

        protected ICarrierDataFactory MetadataGenerator => _dependencyProvider.MetadataGenerator;

        protected IBextProcessor BextProcessor => _dependencyProvider.BextProcessor;

        // ReSharper disable once InconsistentNaming
        protected IFFMPEGRunner FFPMpegRunner => _dependencyProvider.FFMPEGRunner;

        public string BaseSuccessDirectory => ProgramSettings.SuccessDirectoryName;

        public string BaseErrorDirectory => ProgramSettings.ErrorDirectoryName;
        protected virtual string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        public virtual async Task<ValidationResult> ProcessFile(IGrouping<string, AbstractFileModel> fileModels)
        {
            Barcode = fileModels.Key;

            var sectionKey = Observers.BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode);
            try
            {
                // figure out what files we need to touch
                var filesToProcess = GetFilesToProcess(fileModels);

                // now move them to processing
                await CreateProcessingDirectoryAndMoveOriginals(filesToProcess);

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
                return new ValidationResult(e.GetFirstMessage());
            }
        }

        private static List<ObjectFileModel> GetFilesToProcess(IEnumerable<AbstractFileModel> fileModels)
        {
            return fileModels
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsPreservationIntermediateVersion() || m.IsPreservationVersion() || m.IsProductionVersion())
                .ToList();
        }

        protected abstract Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess);

        private async Task CreateProcessingDirectoryAndMoveOriginals(IEnumerable<ObjectFileModel> filesToProcess)
        {
            var sectionKey = Observers.BeginSection("Initializing");
            try
            {
                // make folder to hold processed files
                Observers.Log("Creating folder: {0}", ProcessingDirectory);
                DirectoryProvider.CreateDirectory(ProcessingDirectory);

                if (!ProcessingDirectory.Equals(OriginalsDirectory, StringComparison.InvariantCultureIgnoreCase))
                {
                    Observers.Log("Creating folder: {0}", OriginalsDirectory);
                    DirectoryProvider.CreateDirectory(OriginalsDirectory);
                }

                foreach (var fileModel in filesToProcess)
                {
                    Observers.Log("Moving originals to processing: {0}", fileModel.OriginalFileName);
                    await MoveOriginalToProcessing(fileModel);
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
                Observers.Log("Moving {0} to success folder", ObjectDirectoryName);
                await DirectoryProvider.MoveDirectoryAsync(ProcessingDirectory, GetSafeDirectoryName(BaseSuccessDirectory, ObjectDirectoryName, "success folder"));

                Observers.EndSection(sectionKey, "Cleanup successful");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private string GetSafeDirectoryName(string baseDirectory, string preferredName, string friendlyName)
        {
            var preferredPath = Path.Combine(baseDirectory, preferredName);
            if (DirectoryProvider.DirectoryExists(Path.Combine(baseDirectory, preferredName)) == false)
            {
                return preferredPath;
            }

            Observers.Log("{0} already exists in {1}", preferredName, friendlyName);

            var newName = $"{preferredName}_{DateTime.Now:MMddyyyyhhmmss}";
            preferredPath = Path.Combine(baseDirectory, newName);
            if (DirectoryProvider.DirectoryExists(preferredPath))
            {
                Observers.Log("{0} already exists in {1}", newName, friendlyName);
                throw new FileDirectoryExistsException("{0} already exists in {1}", preferredName, baseDirectory);
            }

            Observers.Log("Using {0} instead.", newName);
            return preferredPath;
        }

        private async Task CopyToDropbox(IEnumerable<AbstractFileModel> fileList)
        {
            var sectionKey = Observers.BeginSection("Copying objects to dropbox");
            try
            {
                if (DirectoryProvider.DirectoryExists(DropBoxDirectory))
                {
                    throw new FileDirectoryExistsException("{0} already exists in dropbox folder", ObjectDirectoryName);
                }

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
                Observers.Log("Moving {0} to error folder", ObjectDirectoryName);

                DirectoryProvider.MoveDirectory(ProcessingDirectory, GetSafeDirectoryName(BaseErrorDirectory, ObjectDirectoryName, "error folder"));
            }
            catch (Exception e)
            {
                Observers.Log("Could not move {0} to error folder: {1}", ObjectDirectoryName, e);
            }
        }

        private async Task<string> MoveOriginalToProcessing(AbstractFileModel fileModel)
        {
            var sourcePath = Path.Combine(InputDirectory, fileModel.OriginalFileName);
            var targetPath = Path.Combine(OriginalsDirectory, fileModel.ToFileName()); // ToFileName will normalize the filename when we move the file

            if (FileProvider.FileExists(targetPath))
            {
                throw new FileDirectoryExistsException("The file {0} already exists in {1}", fileModel.ToFileName(), OriginalsDirectory);
            }

            await FileProvider.MoveFileAsync(sourcePath, targetPath);
            return targetPath;
        }

        protected async Task<T> GetMetadata<T>(List<ObjectFileModel> filesToProcess) where T:AbstractPodMetadata, new()
        {
            var sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
            try
            {
                // get base metadata
                var metadata = await MetadataProvider.GetObjectMetadata<T>(Barcode);
                
                // resolve unit
                metadata.Unit = $"{ProgramSettings.UnitPrefix}{await MetadataProvider.ResolveUnit(metadata.Unit)}";
                
                // log metadata
                MetadataProvider.Log<T>(metadata);

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

        protected async Task AssignChecksumValues(IEnumerable<ObjectFileModel> models)
        {
            foreach (var model in models)
            {
                model.Checksum = await Hasher.Hash(model);
                Observers.Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(model.ToFileName()), model.Checksum);
            }
        } 

    }
}