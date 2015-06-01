using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Models;
using Packager.Observers;
using Packager.Processors;

namespace Packager.Utilities
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IProgramSettings _programSettings;
        private readonly IUtilityProvider _utilityProvider;
        protected List<IObserver> Observers { get; private set; }
        protected abstract string ProductionFileExtension { get; }
        protected abstract string AccessFileExtension { get; }
        protected abstract string MezzanineFileExtension { get; }
        protected abstract string PreservationFileExtension { get; }

        // constructor
        protected AbstractProcessor(IProgramSettings programSettings, IUtilityProvider utilityProvider, List<IObserver> observers)
        {
            _programSettings = programSettings;
            _utilityProvider = utilityProvider;
            Observers = observers;
        }

        protected IExcelImporter ExcelImporter { get { return _utilityProvider.CarrierDataExcelImporter; } }
        protected IBextDataProvider BextDataProvider { get { return _utilityProvider.BextDataProvider; } }
        protected IHasher Hasher { get { return _utilityProvider.Hasher; } }
        protected IUserInfoResolver UserInfoResolver { get { return _utilityProvider.UserInfoResolver; } }
        protected IXmlExporter XmlExporter { get { return _utilityProvider.XmlExporter; } }

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

        public abstract void ProcessFile(IGrouping<string, FileModel> batchGrouping);
        public abstract List<FileModel> ProcessFile(FileModel fileModel);

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
            File.Move(sourcePath, targetPath);
            return targetPath;
        }

        protected FileModel ToAccessFileModel(FileModel original)
        {
            return original.ToAccessFileModel(AccessFileExtension);
        }

        protected FileModel ToMezzanineFileModel(FileModel original)
        {
            return original.ToMezzanineFileModel(MezzanineFileExtension);
        }

        protected FileModel ToProductionFileModel(FileModel original)
        {
            return original.ToProductionFileModel(ProductionFileExtension);
        }
    }
}