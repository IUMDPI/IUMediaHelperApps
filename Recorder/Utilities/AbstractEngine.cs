using System;
using System.Diagnostics;
using Recorder.Models;

namespace Recorder.Utilities
{
    public abstract class AbstractEngine : IDisposable
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
                    CreateNoWindow = true
                }
            };
        }

        protected ProgramSettings Settings { get; }
        protected ObjectModel ObjectModel { get; }

        public virtual void Dispose()
        {
            Process?.Dispose();
        }
    }
}