using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.ResultModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Images;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;
using Packager.Validators;

namespace Packager.Processors
{
    public abstract class AbstractProcessor<T> : IProcessor where T : AbstractPodMetadata, new()
    {
        protected AbstractProcessor(
           IBextProcessor bextProcessor,
           IDirectoryProvider directoryProvider,
           IFileProvider fileProvider,
           IHasher hasher,
           IPodMetadataProvider metadataProvider,
           IObserverCollection observers,
           IProgramSettings programSettings,
           IXmlExporter xmlExporter,
           ILabelImageImporter imageProcessor, IPlaceHolderFactory placeHolderFactory)
        {
            ProgramSettings = programSettings;
            Observers = observers;
            MetadataProvider = metadataProvider;
            XmlExporter = xmlExporter;
            LabelImageImporter = imageProcessor;
            PlaceHolderFactory = placeHolderFactory;
            BextProcessor = bextProcessor;
            DirectoryProvider = directoryProvider;
            FileProvider = fileProvider;
            Hasher = hasher;

            ProjectCode = programSettings.ProjectCode;

            InputDirectory = programSettings.InputDirectory;
            RootDropBoxDirectory = programSettings.DropBoxDirectoryName;
            RootProcessingDirectory = programSettings.ProcessingDirectory;
            BaseErrorDirectory = programSettings.ErrorDirectoryName;
            BaseSuccessDirectory = programSettings.SuccessDirectoryName;
        }

        protected string Barcode { get; private set; }
        private string ProjectCode { get; }

        protected abstract string OriginalsDirectory { get; }

        private IProgramSettings ProgramSettings { get; }
        protected IObserverCollection Observers { get; }
        private IPodMetadataProvider MetadataProvider { get; }
        private IXmlExporter XmlExporter { get; }
        protected ILabelImageImporter LabelImageImporter { get; }
        private IPlaceHolderFactory PlaceHolderFactory { get; }
        private IHasher Hasher { get; }
        private IFileProvider FileProvider { get; }
        private IDirectoryProvider DirectoryProvider { get; }
        protected IBextProcessor BextProcessor { get; }

        private string InputDirectory { get; }
        private string RootDropBoxDirectory { get; }
        private string RootProcessingDirectory { get; }
        private string BaseSuccessDirectory { get; }
        private string BaseErrorDirectory { get; }

        private string ObjectDirectoryName => $"{ProjectCode.ToUpperInvariant()}_{Barcode}";
        protected string ProcessingDirectory => Path.Combine(RootProcessingDirectory, ObjectDirectoryName);
        private string DropBoxDirectory => Path.Combine(RootDropBoxDirectory, ObjectDirectoryName);

        protected abstract ICarrierDataFactory<T> CarrierDataFactory { get; }
        protected abstract IFFMPEGRunner FFMpegRunner { get; }
        protected abstract IEmbeddedMetadataFactory<T> EmbeddedMetadataFactory { get; }
        protected abstract AbstractFile CreateProdOrMezzModel(AbstractFile master);
        protected abstract IEnumerable<AbstractFile> GetProdOrMezzModels(IEnumerable<AbstractFile> models);
        protected abstract Task ClearMetadataFields(List<AbstractFile> processedList, CancellationToken cancellationToken);
        protected abstract Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList, CancellationToken cancellationToken);
        protected abstract ValidationResult ContinueProcessingObject(AbstractPodMetadata metadata);

        public virtual async Task<DurationResult> ProcessObject(IGrouping<string, AbstractFile> fileModels, CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            Barcode = fileModels.Key;

            var sectionKey = Observers.BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode);
            try
            {
                cancellationToken.Register(cancellationToken.ThrowIfCancellationRequested);

                // convert grouping to simple list
                var filesToProcess = fileModels.ToList();

                // fetch, log, and validate metadata
                var metadata = await GetMetadata<T>(filesToProcess, cancellationToken);

                // now move files to processing
                await CreateProcessingDirectoryAndMoveOriginals(filesToProcess, cancellationToken);

                // Should we defer processing
                var shouldContinue = ContinueProcessingObject(metadata);
                if (shouldContinue.Result == false)
                {
                    await ReturnOriginalsToInputHopper(filesToProcess, cancellationToken);
                    return DurationResult.Deferred(DateTime.Now, shouldContinue.Issue);
                }

                // normalize originals
                await NormalizeOriginals(filesToProcess, metadata, cancellationToken);

                // verify normalized versions of originals
                await FFMpegRunner.Verify(filesToProcess, cancellationToken);

                // create list of files to process and add the original files that
                // we know about
                var processedList = new List<AbstractFile>().Concat(filesToProcess)
                    .GroupBy(m => m.Filename).Select(g => g.First()).ToList();

                // next group by sequence
                // then determine which file to use to create derivatives
                // then use that file to create the derivatives
                // then aggregate the results into the processed list
                processedList = processedList.Concat(
                    await CreateProdOrMezzDerivatives(processedList, metadata, cancellationToken)).ToList();

                // now remove duplicate entries -- this could happen if production master
                // already exists
                processedList = processedList.RemoveDuplicates();

                // now clear the ISFT field from presentation and production/mezz masters
                await ClearMetadataFields(processedList, cancellationToken);

                // generate the access versions from production masters
                processedList = processedList.Concat(
                    await CreateAccessDerivatives(processedList, cancellationToken)).ToList();

                // create QC files
                // add qc files to processed list
                processedList = processedList.Concat(
                    await CreateQualityControlFiles(processedList, cancellationToken)).ToList();

                // import label images and add them to list of files to process
                processedList = processedList
                    .Concat(await ImportLabelImages(cancellationToken))
                    .ToList();

                // add place-holder file entries
                processedList = processedList
                    .Concat(GetPlaceHoldersToAdd(metadata.Format, processedList))
                    .ToList();

                // using the list of files that have been processed
                // make the xml file
                processedList.Add(await GenerateXml(metadata, processedList, cancellationToken));

                // copy processed files to drop box
                await CopyToDropbox(processedList, cancellationToken);

                // move everything to success folder
                await MoveToSuccessFolder();

                Observers.EndSection(sectionKey, $"Object processed succesfully: {Barcode}");
                return DurationResult.Success(startTime);
            }
            catch (OperationCanceledException e)
            {
                Observers.LogProcessingIssue(new UserCancelledException(), Barcode);
                MoveToErrorFolder();
                Observers.EndSection(sectionKey);
                return new DurationResult(startTime, e.GetBaseMessage());
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                MoveToErrorFolder();
                Observers.EndSection(sectionKey);
                return new DurationResult(startTime, e.GetBaseMessage());
            }
        }
        
        private async Task NormalizeOriginals(List<AbstractFile> originals, T podMetadata, CancellationToken cancellationToken)
        {
            foreach (var original in originals.Where(f => f is TiffImageFile == false))
            {
                var metadata = EmbeddedMetadataFactory.Generate(originals, original, podMetadata);
                await FFMpegRunner.Normalize(original, metadata, cancellationToken);
            }
        }

        private async Task<List<AbstractFile>> CreateProdOrMezzDerivatives(List<AbstractFile> models, T podMetadata, CancellationToken cancellationToken)
        {
            var results = new List<AbstractFile>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = CreateProdOrMezzModel(master);
                var metadata = EmbeddedMetadataFactory.Generate(models, derivative, podMetadata);
                results.Add(await FFMpegRunner.CreateProdOrMezzDerivative(master, derivative, metadata, cancellationToken));
            }

            return results;
        }

        private async Task<List<AbstractFile>> CreateAccessDerivatives(IEnumerable<AbstractFile> models, CancellationToken cancellationToken)
        {
            var results = new List<AbstractFile>();

            // for each production master, create an access version
            foreach (var model in GetProdOrMezzModels(models))
            {
                results.Add(await FFMpegRunner.CreateAccessDerivative(model, cancellationToken));
            }
            return results;
        }

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

        private async Task<XmlFile> GenerateXml(T metadata, List<AbstractFile> filesToProcess, CancellationToken cancellationToken)
        {
            var result = new XmlFile(ProjectCode, Barcode);
            var sectionKey = Observers.BeginSection("Generating {0}", result.Filename);
            try
            {
                await AssignChecksumValues(filesToProcess, cancellationToken);

                var wrapper = new ExportableManifest
                {
                    Carrier = await CarrierDataFactory.Generate(metadata, ProgramSettings.DigitizingEntity, filesToProcess, cancellationToken)
                };
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.Filename));

                result.Checksum = await Hasher.Hash(result, cancellationToken);
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

        private async Task<List<AbstractFile>> ImportLabelImages(CancellationToken cancellationToken)
        {
            var sectionKey = Observers.BeginSection("Importing Label Images");
            try
            {
                var results = await LabelImageImporter.ImportMediaImages(Barcode, cancellationToken);
                Observers.Log(results.Any()
                    ? string.Join("\n", results.Select(f => $"imported {f.Filename}"))
                    : "No label images found");

                Observers.EndSection(sectionKey);
                return results;
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
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

        private async Task CopyToDropbox(IEnumerable<AbstractFile> fileList, CancellationToken cancellationToken)
        {
            var sectionKey = Observers.BeginSection("Copying objects to dropbox");
            try
            {
                if (DirectoryProvider.DirectoryExists(DropBoxDirectory))
                {
                    throw new FileDirectoryExistsException("{0} already exists in dropbox folder", ObjectDirectoryName);
                }

                DirectoryProvider.CreateDirectory(DropBoxDirectory);

                var toCopy = fileList
                    .NonPlaceHolderFiles()
                    .Select(fileModel => fileModel.Filename)
                    .OrderBy(f => f);

                foreach (var fileName in toCopy)
                {
                    Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                    await FileProvider.CopyFileAsync(
                        Path.Combine(ProcessingDirectory, fileName),
                        Path.Combine(DropBoxDirectory, fileName), cancellationToken);
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

        private async Task CreateProcessingDirectoryAndMoveOriginals(IEnumerable<AbstractFile> filesToProcess, CancellationToken cancellationToken)
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
                    await MoveOriginalToProcessing(fileModel, cancellationToken);
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

        private async Task<string> MoveOriginalToProcessing(AbstractFile fileModel, CancellationToken cancellationToken)
        {
            var sourcePath = Path.Combine(InputDirectory, fileModel.OriginalFileName);
            var targetPath = Path.Combine(OriginalsDirectory, fileModel.Filename);

            return await MoveFile(sourcePath, targetPath, cancellationToken);
        }

        private async Task ReturnOriginalsToInputHopper(IEnumerable<AbstractFile> filesToProcess, CancellationToken cancellationToken)
        {
            var sectionKey = Observers.BeginSection("Returning object files to input folder");
            try
            {
                foreach (var fileModel in filesToProcess)
                {
                    await ReturnOriginalToInputHopper(fileModel, cancellationToken);
                }
            }
            finally
            {
                Observers.EndSection(sectionKey);
            }
            
        }

        private async Task ReturnOriginalToInputHopper(AbstractFile fileModel, CancellationToken cancellationToken)
        {
            Observers.Log("Returning {0} to input hopper", fileModel.Filename);
            var sourcePath = Path.Combine(OriginalsDirectory, fileModel.OriginalFileName);
            var targetPath = Path.Combine(InputDirectory, fileModel.Filename);

            await MoveFile(sourcePath, targetPath, cancellationToken);
        }

        private async Task<string> MoveFile(string sourcePath, string targetPath, CancellationToken cancellationToken)
        {

            if (FileProvider.FileExists(targetPath))
            {
                throw new FileDirectoryExistsException("The file {0} already exists in {1}", Path.GetFileName(targetPath),
                    OriginalsDirectory);
            }

            await FileProvider.MoveFileAsync(sourcePath, targetPath, cancellationToken);
            return targetPath;
        }

        private static string GetFilenameToLog(AbstractFile model)
        {
            return model.OriginalFileName.Equals(model.Filename, StringComparison.InvariantCultureIgnoreCase)
                ? model.Filename
                : $"{model.Filename} ({model.OriginalFileName})";
        }

        private async Task<TMetadataType> GetMetadata<TMetadataType>(List<AbstractFile> filesToProcess, CancellationToken cancellationToken) where TMetadataType : AbstractPodMetadata, new()
        {
            var sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
            try
            {
                // get base metadata
                var metadata = await MetadataProvider.GetObjectMetadata<TMetadataType>(Barcode, cancellationToken);

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

        private async Task AssignChecksumValues(IEnumerable<AbstractFile> models, CancellationToken cancellationToken)
        {
            // hash every model, but place holders
            foreach (var model in models.NonPlaceHolderFiles())
            {
                model.Checksum = await Hasher.Hash(model, cancellationToken);
                Observers.Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(model.Filename), model.Checksum);
            }
        }

        private IEnumerable<AbstractFile> GetPlaceHoldersToAdd(IMediaFormat format, List<AbstractFile> processedList)
        {
            var sectionKey = Observers.BeginSection("Adding Placeholder Entries");
            try
            {
                var toAdd = PlaceHolderFactory.GetPlaceHoldersToAdd(format, processedList);
                if (toAdd.Any() == false)
                {
                    Observers.Log("No placeholders to add");
                }
                else
                {
                    foreach (var entry in toAdd)
                    {
                        Observers.Log("Adding placeholder: {0}", entry.Filename);
                    }
                }

                Observers.EndSection(sectionKey);
                return toAdd;

            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, Barcode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }

        }
    }
}