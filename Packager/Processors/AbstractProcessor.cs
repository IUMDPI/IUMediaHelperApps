using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        protected List<IObserver> Observers
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

        protected IBextDataProvider BextDataProvider
        {
            get { return _dependencyProvider.BextDataProvider; }
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

        protected string Barcode { get; set; }

        protected string ProjectCode
        {
            get { return ProgramSettings.ProjectCode; }
        }

        protected string ProcessingDirectory
        {
            get { return Path.Combine(RootProcessingDirectory, string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode)); }
        }

        protected string DropBoxDirectory
        {
            get { return Path.Combine(RootDropBoxDirectory, string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode)); }
        }

        public abstract Task ProcessFile(IGrouping<string, AbstractFileModel> barcodeGrouping);
        public abstract Task<List<ObjectFileModel>> CreateDerivatives(ObjectFileModel fileModel);
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

        protected string MoveFileToProcessing(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid file name", "fileName");
            }

            var sourcePath = Path.Combine(InputDirectory, fileName);
            var targetPath = Path.Combine(ProcessingDirectory, fileName);
            FileProvider.Move(sourcePath, targetPath);
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

        protected async Task<PodMetadata> GetMetadata()
        {
            var metadata = await MetadataProvider.Get(Barcode);
            if (!metadata.Success)
            {
                throw new Exception(string.Format("Could not retrieve metadata: {0}", metadata.Message.ToDefaultIfEmpty("[no error message present]")));
            }

            return metadata;
        }
    }
}