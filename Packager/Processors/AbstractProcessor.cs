using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Processors
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IProgramSettings _programSettings;
        private readonly IDependencyProvider _dependencyProvider;
        protected List<IObserver> Observers { get; private set; }
        protected abstract string ProductionFileExtension { get; }
        protected abstract string AccessFileExtension { get; }
        protected abstract string MezzanineFileExtension { get; }
        protected abstract string PreservationFileExtension { get; }

        // constructor
        protected AbstractProcessor(IProgramSettings programSettings, IDependencyProvider dependencyProvider, List<IObserver> observers)
        {
            _programSettings = programSettings;
            _dependencyProvider = dependencyProvider;
            Observers = observers;
        }

        protected IExcelImporter ExcelImporter { get { return _dependencyProvider.CarrierDataExcelImporter; } }
        protected IBextDataProvider BextDataProvider { get { return _dependencyProvider.BextDataProvider; } }
        protected IHasher Hasher { get { return _dependencyProvider.Hasher; } }
        protected IUserInfoResolver UserInfoResolver { get { return _dependencyProvider.UserInfoResolver; } }
        protected IXmlExporter XmlExporter { get { return _dependencyProvider.XmlExporter; } }

        protected string Barcode { get; set; }

        protected string ProjectCode
        {
            get { return _programSettings.ProjectCode; }
        }

        protected string ProcessingDirectory
        {
            get { return Path.Combine(RootProcessingDirectory, string.Format("{0}_{1}", ProjectCode, Barcode)); }
        }

        protected string DropBoxDirectory
        {
            get { return Path.Combine(RootDropBoxDirectory, string.Format("{0}_{1}", ProjectCode, Barcode)); }
        }

        public abstract void ProcessFile(IGrouping<string, AbstractFileModel> batchGrouping);
        public abstract List<ObjectFileModel> ProcessFile(ObjectFileModel objectFileModelModel);

        // ReSharper disable once InconsistentNaming
        protected string BWFMetaEditPath
        {
            get { return _programSettings.BWFMetaEditPath; }
        }

        protected string DataFormat
        {
            get { return _programSettings.DateFormat; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGPath
        {
            get { return _programSettings.FFMPEGPath; }
        }

        private string InputDirectory
        {
            get { return _programSettings.InputDirectory; }
        }

        private string RootDropBoxDirectory
        {
            get { return _programSettings.DropBoxDirectoryName; }
        }

        private string RootProcessingDirectory
        {
            get { return _programSettings.ProcessingDirectory; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioMezzanineArguments
        {
            get { return _programSettings.FFMPEGAudioMezzanineArguments; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGAudioAccessArguments
        {
            get { return _programSettings.FFMPEGAudioAccessArguments; }
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

        protected IFileProvider FileProvider { get { return _dependencyProvider.FileProvider; } }
        protected IDirectoryProvider DirectoryProvider { get { return _dependencyProvider.DirectoryProvider; } }
    }
}