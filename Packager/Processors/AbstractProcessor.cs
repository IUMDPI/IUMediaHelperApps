﻿using System;
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
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Process;
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

        private IXmlExporter XmlExporter => DependencyProvider.XmlExporter;

        private string ProjectCode => ProgramSettings.ProjectCode;

        private string ObjectDirectoryName => $"{ProjectCode.ToUpperInvariant()}_{Barcode}";

        protected string ProcessingDirectory => Path.Combine(RootProcessingDirectory, ObjectDirectoryName);

        private string DropBoxDirectory => Path.Combine(RootDropBoxDirectory, ObjectDirectoryName);

        protected string Barcode { get; private set; }

        private string InputDirectory => ProgramSettings.InputDirectory;

        private string RootDropBoxDirectory => ProgramSettings.DropBoxDirectoryName;

        private string RootProcessingDirectory => ProgramSettings.ProcessingDirectory;

        private IHasher Hasher => DependencyProvider.Hasher;

        private IFileProvider FileProvider => DependencyProvider.FileProvider;

        private IDirectoryProvider DirectoryProvider => DependencyProvider.DirectoryProvider;

        protected IBextProcessor BextProcessor => DependencyProvider.BextProcessor;

        private string BaseSuccessDirectory => ProgramSettings.SuccessDirectoryName;

        private string BaseErrorDirectory => ProgramSettings.ErrorDirectoryName;

        protected abstract string OriginalsDirectory { get; }

        protected abstract ICarrierDataFactory CarrierDataFactory { get; }

        protected abstract IFFMPEGRunner FFMpegRunner { get; }

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

                // fetch, log, and validate metadata
                var metadata = await GetMetadata(filesToProcess);

                // normalize originals
                await NormalizeOriginals(filesToProcess, metadata);

                // verify normalized versions of originals
                await FFMpegRunner.Verify(filesToProcess);

                // create list of files to process and add the original files that
                // we know about
                var processedList = new List<AbstractFile>().Concat(filesToProcess)
                    .GroupBy(m => m.Filename).Select(g => g.First()).ToList();

                // next group by sequence
                // then determine which file to use to create derivatives
                // then use that file to create the derivatives
                // then aggregate the results into the processed list
                processedList =
                    processedList.Concat(await CreateProdOrMezzDerivatives(processedList, metadata)).ToList();

                // now remove duplicate entries -- this could happen if production master
                // already exists
                processedList = processedList.RemoveDuplicates();

                // now clear the ISFT field from presentation and production/mezz masters
                await ClearMetadataFields(processedList);

                // generate the access versions from production masters
                processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();

                // create QC files
                var qcFiles = await CreateQualityControlFiles(processedList);
                // add qc files to processed list
                processedList = processedList.Concat(qcFiles).ToList();

                // using the list of files that have been processed
                // make the xml file
                var xmlModel = await GenerateXml(metadata, processedList);
                processedList.Add(xmlModel);

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

        protected abstract Task<AbstractPodMetadata> GetMetadata(List<AbstractFile> filesToProcess);

        protected abstract Task NormalizeOriginals(List<AbstractFile> originals, AbstractPodMetadata podMetadata);

        protected abstract Task<List<AbstractFile>> CreateProdOrMezzDerivatives(List<AbstractFile> models,
            AbstractPodMetadata metadata);

        protected abstract Task ClearMetadataFields(List<AbstractFile> processedList);

        protected abstract Task<List<AbstractFile>> CreateQualityControlFiles(List<AbstractFile> processedList);


        private async Task<List<AbstractFile>> CreateAccessDerivatives(IEnumerable<AbstractFile> models)
        {
            var results = new List<AbstractFile>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsMezzanineVersion()))
            {
                var test = await FFMpegRunner.CreateAccessDerivative(model);
                results.Add(await FFMpegRunner.CreateAccessDerivative(model));
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

        private async Task<XmlFile> GenerateXml(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            var result = new XmlFile(ProjectCode, Barcode);
            var sectionKey = Observers.BeginSection("Generating {0}", result.Filename);
            try
            {
                await AssignChecksumValues(filesToProcess);

                var wrapper = new IU {Carrier = CarrierDataFactory.Generate(metadata, filesToProcess)};
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
                        throw new Exception(
                            $"Could not validate file {model.Filename}: checksum ({checksum}) not same as expected ({model.Checksum})");
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
            return model.OriginalFileName.Equals(model.Filename, StringComparison.InvariantCultureIgnoreCase)
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