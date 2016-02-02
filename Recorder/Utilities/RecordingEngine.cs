using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.VisualBasic.FileIO;
using Recorder.Handlers;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public class RecordingEngine : AbstractEngine
    {
        private readonly InfoEngine _infoEngine;
        private readonly VolumeMeter _volumeMeter;
        private bool _recording;

        public RecordingEngine(IProgramSettings settings, ObjectModel objectModel, OutputWindowViewModel outputModel, IssueNotifyModel issueNotifyModel)
            : base(settings, objectModel, outputModel, issueNotifyModel)
        {
            Recording = false;
            CumulativeTimeSpan = new TimeSpan();
            Process.Exited += ProcessExitHandler;

            ProcessObservers.Add(new TimestampReceivedHandler(this));

            _infoEngine = new InfoEngine(settings);
            _volumeMeter = new VolumeMeter(settings);
            _volumeMeter.SampleAggregator.MaximumCalculated += MaximumCalculatedHandler;
        }

        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan CumulativeTimeSpan { get; private set; }

        private void MaximumCalculatedHandler(object sender, MaxSampleEventArgs e)
        {
            var lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
            OutputWindowViewModel.VolumeMeterViewModel.InputLevel = lastPeak*100;
        }

        public event EventHandler<TimeSpan> TimestampUpdated;

        

        protected override ValidationResult IsGlobalConfigurationOk()
        {
            var baseResult = base.IsGlobalConfigurationOk();
            if (!baseResult.IsValid)
            {
                return baseResult;
            }

            if (!File.Exists(Settings.PathToFFMPEG))
            {
                return new ValidationResult(false, "invalid FFMPEG path");
            }

            if (!File.Exists(Settings.PathToFFProbe))
            {
                return new ValidationResult(false, "invalid FFProbe path");
            }

            return ValidationResult.ValidResult;
        }

        public async Task StartRecording()
        {
            if (Recording)
            {
                return;
            }

            Recording = true;


            if (!Directory.Exists(ObjectModel.WorkingFolderPath))
            {
                Directory.CreateDirectory(ObjectModel.WorkingFolderPath);
            }


            ResetObservers();
            var part = GetNewPart();

            OutputWindowViewModel.StartOutput($"Recording {GetTargetPartFilename(part)}");
            OutputWindowViewModel.ShowVolumeMeter();

            _volumeMeter.StartMonitoring();

            CumulativeTimeSpan = await GetDurationOfExistingParts();

            Process.StartInfo.Arguments = $"{Settings.FFMPEGArguments} -ac {ObjectModel.Channels} {GetTargetPartFilename(part)}";

            Process.StartInfo.EnvironmentVariables["FFREPORT"] = $"file={GetTargetPartLogFilename(part)}:level=32";
            Process.StartInfo.WorkingDirectory = ObjectModel.WorkingFolderPath;

            Process.Start();

            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        public void StopRecording()
        {
            if (!Recording)
            {
                return;
            }

            Process.StandardInput.WriteLine('q');
        }

        private void ProcessExitHandler(object sender, EventArgs e)
        {
            try
            {
                if (!Dispatcher.CurrentDispatcher.CheckAccess())
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => ProcessExitHandler(sender, e));
                    return;
                }


                Recording = false;
                _volumeMeter.StopMonitoring();
                OutputWindowViewModel.HideVolumeMeter();
                CheckExitCode();
            }
            finally
            {
                Process.CancelErrorRead();
                Process.CancelOutputRead();
            }
        }

        public void ClearExistingParts()
        {
            if (OkToProcess().IsValid == false)
            {
                return;
            }

            foreach (var file in GetExistingParts())
            {
                FileSystem.DeleteFile(file, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
        }

        private int GetNewPart()
        {
            var count = 0;
            string value;
            do
            {
                count++;
                value = GetTargetPartFilename(count);
                Thread.Sleep(5);
            } while (File.Exists(Path.Combine(ObjectModel.WorkingFolderPath, value)));


            return count;
        }

        private IEnumerable<string> GetExistingParts()
        {
            return Directory.Exists(ObjectModel.WorkingFolderPath)
                ? Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, ObjectModel.ExistingPartsMask)
                : new List<string>();
        }

        private async Task<TimeSpan> GetDurationOfExistingParts()
        {
            if (OkToProcess().IsValid == false)
            {
                return new TimeSpan();
            }

            var result = new TimeSpan();
            foreach (var part in GetExistingParts())
            {
                result = result.Add(await _infoEngine.GetDuration(part));
            }

            return result;
        }

        public async Task<TimeSpan> ResetCumulativeTimestamp()
        {
            CumulativeTimeSpan = await GetDurationOfExistingParts();
            return CumulativeTimeSpan;
        }

        private string GetTargetPartFilename(int part)
        {
            return $"{ObjectModel.ProjectCode}_{ObjectModel.Barcode}_part_{part:d5}.mkv";
        }


        private string GetTargetPartLogFilename(int part)
        {
            return $"{ObjectModel.ProjectCode}_{ObjectModel.Barcode}_part_{part:d5}.log";
        }

        public virtual void OnTimestampUpdated(TimeSpan e)
        {
            TimestampUpdated?.Invoke(this, e);
        }

        public override void Dispose()
        {
            StopRecording();
            _volumeMeter.Dispose();
            base.Dispose();
        }

        protected override void CheckExitCode()
        {
            if (Process.ExitCode == 0)
            {
                return;
            }

            IssueNotifyModel.Notify(GetErrorMessageFromOutput("Something went wrong while recording parts: {0}"));
        }

        public override void LogConfiguration(OutputWindowViewModel outputModel)
        {
            var globalConfigOk = IsGlobalConfigurationOk();
            if (globalConfigOk.IsValid)
            {
                outputModel.WriteLine("Recorder: initialized");
            }
            else
            {
                outputModel.WriteLine("Recorder: not initialized");
                outputModel.WriteLine($"Recorder: {globalConfigOk.ErrorContent}");
                outputModel.WriteLine("");
            }

            _volumeMeter.LogConfiguration(outputModel);
        }
    }
}