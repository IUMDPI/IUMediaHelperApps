using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Recorder.Handlers;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public abstract class AbstractEngine : IDisposable, INotifyPropertyChanged
    {
        protected readonly Process Process;
        private OutputWindowViewModel _outputWindowViewModel;

        protected AbstractEngine(IProgramSettings settings, ObjectModel objectModel, OutputWindowViewModel outputModel)
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

            OutputWindowViewModel = outputModel;
            OutputReceivedHandler = new OutputReceivedHandler(OutputWindowViewModel);

            Process.OutputDataReceived += OutputReceivedHandler.OnDataReceived;
            Process.ErrorDataReceived += OutputReceivedHandler.OnDataReceived;
        }


        protected OutputReceivedHandler OutputReceivedHandler { get;}
        protected OutputWindowViewModel OutputWindowViewModel { get; }


        protected IProgramSettings Settings { get; }
        protected ObjectModel ObjectModel { get; }
        
        public virtual void Dispose()
        {
            Process?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}