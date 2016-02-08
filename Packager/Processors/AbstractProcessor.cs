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
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Xml;
using Packager.Validators;

namespace Packager.Processors
{
    public abstract class AbstractProcessor : IProcessor
    {
        // constructor
        protected AbstractProcessor(IDependencyProvider dependencyProvider)
        {
            DependencyProvider = dependencyProvider;
        }

        protected IDependencyProvider DependencyProvider { get; }

        private IProgramSettings ProgramSettings => DependencyProvider.ProgramSettings;

        protected IObserverCollection Observers => DependencyProvider.Observers;

        private IPodMetadataProvider MetadataProvider => DependencyProvider.MetadataProvider;

        protected IXmlExporter XmlExporter => DependencyProvider.XmlExporter;

        protected string ProjectCode => ProgramSettings.ProjectCode;

        private string ObjectDirectoryName => $"{ProjectCode.ToUpperInvariant()}_{Barcode}";

        protected string ProcessingDirectory => Path.Combine(RootProcessingDirectory, ObjectDirectoryName);

        private string DropBoxDirectory => Path.Combine(RootDropBoxDirectory, ObjectDirectoryName);

        protected string Barcode { get; private set; }

        protected string InputDirectory => ProgramSettings.InputDirectory;

        private string RootDropBoxDirectory => ProgramSettings.DropBoxDirectoryName;

        private string RootProcessingDirectory => ProgramSettings.ProcessingDirectory;

        protected IHasher Hasher => DependencyProvider.Hasher;

        protected IFileProvider FileProvider => DependencyProvider.FileProvider;

        protected IDirectoryProvider DirectoryProvider => DependencyProvider.DirectoryProvider;

        protected ICarrierDataFactory MetadataGenerator => DependencyProvider.MetadataGenerator;

        protected IBextProcessor BextProcessor => DependencyProvider.BextProcessor;

        private string BaseSuccessDirectory => ProgramSettings.SuccessDirectoryName;

        private string BaseErrorDirectory => ProgramSettings.ErrorDirectoryName;

        protected abstract string OriginalsDirectory { get; }

        public virtual async Task<ValidationResult> ProcessFile(IGrouping<string, AbstractFile> fileModels)
        {
            Barcode = fileModels.Key;

            var sectionKey = Observers.BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode);
            try
            {
                // convert grouping to simple list
                var filesToProcess = fileModels.ToList();

                // now move them to processing
                await CreateProcessingDirectoryAndMoveOriginals(filesToProcess);

                // call internal implementation
                var processedList = await ProcessFileInternal(filesToProcess);
                
                // copy processed files to drop box
                await CopyToDropbox(processedList);

                // verify that all files make it to dropbox
                await VerifyDropboxFiles(processedList);

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
                return new ValidationResult(e.GetBaseMessage());
            }
        }

        protected abstract Task<List<AbstractFile>> ProcessFileInternal(List<AbstractFile> filesToProcess);

        private async Task MoveToSuccessFolder()
        {
            var sectionKey = Observers.BeginSection("Cleaning up");
            try
            {
                Observers.Log("Moving {0} to success folder", ObjectDirectoryName);
                await
                    DirectoryProvider.MoveDirectoryAsync(ProcessingDirectory,
                        GetSafeDirectoryName(BaseSuccessDirectory, ObjectDirectoryName, "success folder"));

                Observers.EndSection(sectionKey, "Cleanup successful");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        protected async Task<XmlFile> GenerateXml<T>(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess) where T:AbstractCarrierData
        {
            var result = new XmlFile(ProjectCode, Barcode);
            var sectionKey = Observers.BeginSection("Generating {0}", result.Filename);
            try
            {
                await AssignChecksumValues(filesToProcess);

                var wrapper = new IU { Carrier = MetadataGenerator.Generate<T>(metadata, filesToProcess) };
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.Filename));

                result.Checksum = await Hasher.Hash(result);
                Observers.Log("{0} checksum: {1}", result.Filename, result.Checksum);

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

        private async Task VerifyDropboxFiles(IEnumerable<AbstractFile> processedList)
        {
            var sectionKey = Observers.BeginSection("Validating dropbox files");
            try
            {
                foreach (var model in processedList)
                {
                    var dropboxPath = Path.Combine(DropBoxDirectory, model.Filename);
                    var checksum = await Hasher.Hash(dropboxPath);
                    if (model.Checksum.Equals(checksum))
                    {
                        Observers.Log("{0}: {1}", model.Filename, checksum);
                    }
                    else
                    {
                        throw new Exception($"Could not validate file {model.Filename}: checksum ({checksum}) not same as expected ({model.Checksum})");
                    }
                }

                Observers.EndSection(sectionKey, "Dropbox files validated successfully!");
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

        private async Task CopyToDropbox(IEnumerable<AbstractFile> fileList)
        {
            var sectionKey = Observers.BeginSection("Copying objects to dropbox");
            try
            {
                if (DirectoryProvider.DirectoryExists(DropBoxDirectory))
                {
                    throw new FileDirectoryExistsException("{0} already exists in dropbox folder", ObjectDirectoryName);
                }

                DirectoryProvider.CreateDirectory(DropBoxDirectory);

                foreach (var fileName in fileList.Select(fileModel => fileModel.Filename).OrderBy(f => f))
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

                DirectoryProvider.MoveDirectory(ProcessingDirectory,
                    GetSafeDirectoryName(BaseErrorDirectory, ObjectDirectoryName, "error folder"));
            }
            catch (Exception e)
            {
                Observers.Log("Could not move {0} to error folder: {1}", ObjectDirectoryName, e);
            }
        }

        private async Task CreateProcessingDirectoryAndMoveOriginals(IEnumerable<AbstractFile> filesToProcess)
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
                    Observers.Log("Moving originals to processing: {0}", GetFilenameToLog(fileModel));
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

        private static string GetFilenameToLog(AbstractFile model)
        {
            return (model.OriginalFileName.Equals(model.Filename, StringComparison.InvariantCultureIgnoreCase))
                ? model.Filename
                : $"{model.Filename} ({model.OriginalFileName})";
        }

        private async Task<string> MoveOriginalToProcessing(AbstractFile fileModel)
        {
            var sourcePath = Path.Combine(InputDirectory, fileModel.OriginalFileName);
            var targetPath = Path.Combine(OriginalsDirectory, fileModel.Filename);
            
            if (FileProvider.FileExists(targetPath))
            {
                throw new FileDirectoryExistsException("The file {0} already exists in {1}", fileModel.Filename,
                    OriginalsDirectory);
            }

            await FileProvider.MoveFileAsync(sourcePath, targetPath);
            return targetPath;
        }

        protected async Task<T> GetMetadata<T>(List<AbstractFile> filesToProcess) where T : AbstractPodMetadata, new()
        {
            var sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
            try
            {
                // get base metadata
                var metadata = await MetadataProvider.GetObjectMetadata<T>(Barcode);

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