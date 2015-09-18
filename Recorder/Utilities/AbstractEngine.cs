using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Recorder.Models;

namespace Recorder.Utilities
{
    public abstract class AbstractEngine : IDisposable, INotifyPropertyChanged
    {
        protected readonly Process Process;

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
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
        }

        protected ProgramSettings Settings { get; }
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