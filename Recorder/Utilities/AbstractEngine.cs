using System;
using System.Collections.Generic;
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

        protected readonly List<AbstractProcessOutputHandler> ProcessObservers = new List<AbstractProcessOutputHandler>();

        protected AbstractEngine(IProgramSettings settings, ObjectModel objectModel, OutputWindowViewModel outputModel, IssueNotifyModel issueNotifyModel)
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
            IssueNotifyModel = issueNotifyModel;

            ProcessObservers.Add(new OutputLogHandler(OutputWindowViewModel));
            ProcessObservers.Add(new DroppedFramesHandler(outputModel.FrameStatsViewModel));
            ProcessObservers.Add(new DuplicateFrameHandler(outputModel.FrameStatsViewModel));
            ProcessObservers.Add(new CurrentFrameHandler(outputModel.FrameStatsViewModel));
        }

        protected OutputWindowViewModel OutputWindowViewModel { get; }
        protected IssueNotifyModel IssueNotifyModel { get; set; }


        protected IProgramSettings Settings { get; }
        protected ObjectModel ObjectModel { get; }

        public virtual void Dispose()
        {
            Process?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Initialize()
        {
            foreach (var observer in ProcessObservers)
            {
                Process.OutputDataReceived += observer.OnDataReceived;
                Process.ErrorDataReceived += observer.OnDataReceived;
            }
        }

        public virtual void ResetObservers()
        {
            foreach (var observer in ProcessObservers)
            {
                observer.Reset();
            }
        }

        protected string GetErrorMessageFromOutput(string baseFormat)
        {
            if (string.IsNullOrWhiteSpace(OutputWindowViewModel.Text))
            {
                return string.Format(baseFormat, "[unknown issue]");
            }

            var lines = OutputWindowViewModel.Text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                return string.Format(baseFormat, "[unknown issue]");
            }


            return string.Format(baseFormat, lines[lines.Length - 2]);
        }

        protected abstract void CheckExitCode();


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}