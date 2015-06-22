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

        public abstract Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel);
        protected abstract Task ProcessFileInternal(IEnumerable<AbstractFileModel> fileModels);
        
        private readonly IDependencyProvider _dependencyProvider;
        
        // constructor
        protected AbstractProcessor(string barcode, IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            Barcode = barcode;
        }

        private IProgramSettings ProgramSettings
        {
            get { return _dependencyProvider.ProgramSettings; }
        }

        protected List<IObserver> Observers
        {
            get { return _dependencyProvider.Observers; }
        }

        protected abstract string ProductionFileExtension { get; }
        protected abstract string AccessFileExtension { get; }
        protected abstract string MezzanineFileExtension { get; }
        protected abstract string PreservationFileExtension { get; }
        protected abstract string PreservationIntermediateFileExtenstion { get; }

        public Guid SectionKey { get; private set; }

        protected IPodMetadataProvider MetadataProvider
        {
            get { return _dependencyProvider.MetadataProvider; }
        }

        protected IHasher Hasher
        {
            get { return _dependencyProvider.Hasher; }
        }

        protected IUserInfoResolver UserInfoResolver
        {
            get { return _dependencyProvider.UserInfoResolver; }
        }

        protected IXmlExporter XmlExporter
        {
            get { return _dependencyProvider.XmlExporter; }
        }

        public string Barcode { get; private set; }

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

        protected string DropBoxDirectory
        {
            get { return Path.Combine(RootDropBoxDirectory, string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode)); }
        }

        public virtual async Task<bool> ProcessFile(IEnumerable<AbstractFileModel> fileModels)
        {
            try
            {
                AddObjectProcessingObserver();
                SectionKey = Observers.BeginSection("Processing Object: {0}", Barcode);

                await ProcessFileInternal(fileModels);
                await MoveToSuccessFolder();
                
                return true;
            }
            catch (Exception e)
            {
                Observers.LogError(e);
                MoveToErrorFolder();
                return false;
            }
            finally
            {
                Observers.EndSection(SectionKey);
                RemoveObjectProcessingObservers();
            }
        }
        
        private async Task MoveToSuccessFolder()
        {
            // move folder to success
            Observers.Log("Moving {0} to success directory", ObjectDirectoryName);
            await DirectoryProvider.MoveDirectoryAsync(ProcessingDirectory, SuccesDirectory);
        }

        protected async Task CopyToDropbox(IEnumerable<AbstractFileModel> fileList)
        {
            var sectionId = Guid.Empty;
            var success = false;
            try
            {
                sectionId = Observers.BeginSection("Copying objects to dropbox");
                
                DirectoryProvider.CreateDirectory(DropBoxDirectory);

                
                foreach (var fileName in fileList.Select(fileModel => fileModel.ToFileName()).OrderBy(f => f))
                {
                    Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                    await FileProvider.CopyFileAsync(
                        Path.Combine(ProcessingDirectory, fileName),
                        Path.Combine(DropBoxDirectory, fileName));
                }
                success = true;
            }
            finally
            {
                Observers.EndSection(sectionId);
                if (success)
                {
                    Observers.FlagAsSuccessful(sectionId, string.Format("{0} files copied to dropbox folder successfully", Barcode));
                }
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
                Observers.Log("Could not more {0} to error directory: {1}",ObjectDirectoryName, e);
            }
        }
        
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

        protected async Task<string> MoveFileToProcessing(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid file name", "fileName");
            }

            var sourcePath = Path.Combine(InputDirectory, fileName);
            var targetPath = Path.Combine(ProcessingDirectory, fileName);
            await FileProvider.MoveFileAsync(sourcePath, targetPath);
            return targetPath;
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

        protected async Task<ConsolidatedPodMetadata> GetMetadata()
        {
            var sectionKey = Guid.Empty;
            var success = false;
            try
            {
                sectionKey = Observers.BeginSection("Requesting metadata for object: {0}", Barcode);
                var metadata = await MetadataProvider.Get(Barcode);
                if (!metadata.Success)
                {
                    throw new PodMetadataException(
                        "Could not retrieve metadata: {0}",
                        metadata.Message.ToDefaultIfEmpty("[no error message present]"));
                }

                Observers.LogObjectProperties(metadata);

                success = true;
                return metadata;
            }
            finally
            {
                Observers.EndSection(sectionKey);
                if (success)
                {
                    Observers.FlagAsSuccessful(sectionKey, string.Format("Retrieved metadata for object: {0}", Barcode));
                }
            }
        }
        
        protected void AddObjectProcessingObserver()
        {
            RemoveObjectProcessingObservers();

            Observers.Add(new ObjectProcessingNLogObserver(
                Barcode,
                ProjectCode,
                ProgramSettings.LogDirectoryName,
                ProgramSettings.ProcessingDirectory));
        }

        protected void RemoveObjectProcessingObservers()
        {
            Observers.RemoveAll(o => o is ObjectProcessingNLogObserver);
        }
    }
}