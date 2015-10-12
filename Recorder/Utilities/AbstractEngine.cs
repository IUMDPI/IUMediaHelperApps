using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public abstract class AbstractEngine : IDisposable, INotifyPropertyChanged
    {
        protected readonly Process Process;
        private OutputWindowViewModel _outputWindowViewModel;

        protected AbstractEngine(ProgramSettings settings, ObjectModel objectModel)
        {
            Settings = settings;
            ObjectModel = objectModel;

            Process = new Process
            {
                StartInfo = new ProcessStartInfo(Settings.PathToFFMPEG)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
        }

        protected ProgramSettings Settings { get; }
        protected ObjectModel ObjectModel { get; }

        public OutputWindowViewModel OutputWindowViewModel
        {
            get { return _outputWindowViewModel; }
            set
            {
                _outputWindowViewModel = value;
                ConfigureOutputCapture();
            }
        }

        public virtual void Dispose()
        {
            Process?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract void ConfigureOutputCapture();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}