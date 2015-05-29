using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Models;
using Packager.Observers;

namespace Packager.Utilities
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IProgramSettings _programSettings;
        protected List<IObserver> Observers { get; private set; }

        public abstract List<FileModel> ProcessFile(FileModel fileModel);
        public abstract void ProcessFile(IGrouping<string, FileModel> batchGrouping);
        public abstract FileModel ToAccessFileModel(FileModel original);
        public abstract FileModel ToMezzanineFileModel(FileModel original);

        public abstract string SupportedExtension { get; }

        protected string Barcode { get; set; }
        protected string ProjectCode { get; set; }

        public string ProcessingDirectory
        {
            get { return Path.Combine(RootProcessingDirectory, string.Format("{0}_{1}", ProjectCode, Barcode)); }
        }

        // ReSharper disable once InconsistentNaming
        public string BWFMetaEditPath
        {
            get { return _programSettings.BWFMetaEditPath; }
        }

        // ReSharper disable once InconsistentNaming
        protected string FFMPEGPath
        {
            get { return _programSettings.FFMPEGPath; }
        }

        protected string InputDirectory
        {
            get { return _programSettings.InputDirectory; }
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

        protected AbstractProcessor(IProgramSettings programSettings, List<IObserver> observers)
        {
            _programSettings = programSettings;
            Observers = observers;
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


    }
}