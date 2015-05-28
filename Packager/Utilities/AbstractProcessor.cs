using System;
using System.Collections.Generic;
using System.IO;
using Packager.Models;
using Packager.Observers;

namespace Packager.Utilities
{
    public abstract class AbstractProcessor : IProcessor
    {
        private readonly IProgramSettings _programSettings;
        protected List<IObserver> Observers { get; private set; }
        public abstract void ProcessFile(string targetPath);
        
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

        protected string ProcessingDirectory
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