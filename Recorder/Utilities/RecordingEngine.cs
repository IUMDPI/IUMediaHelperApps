﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.VisualBasic.FileIO;
using Microsoft.WindowsAPICodePack.Shell;
using Recorder.Handlers;
using Recorder.Models;

namespace Recorder.Utilities
{
    public class RecordingEngine : AbstractEngine
    {
        private bool _recording;

        public RecordingEngine(ProgramSettings settings, ObjectModel objectModel) : base(settings, objectModel)
        {
            Recording = false;
            CumulativeTimeSpan = new TimeSpan();
            Process.Exited += ProcessExitHandler;
            TimestampHandler = new TimestampReceivedHandler(this);
            Process.ErrorDataReceived += TimestampHandler.OnDataReceived;
            Process.OutputDataReceived += TimestampHandler.OnDataReceived;
        }


        private TimestampReceivedHandler TimestampHandler { get; }

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
        public event EventHandler<TimeSpan> TimestampUpdated;

        public ValidationResult OkToRecord()
        {
            if (!File.Exists(Settings.PathToFFMPEG))
            {
                return new ValidationResult(false, "invalid FFMPEG path");
            }

            if (!Directory.Exists(Settings.WorkingFolder))
            {
                return new ValidationResult(false, "working folder does not exist");
            }

            if (!Directory.Exists(Settings.OutputFolder))
            {
                return new ValidationResult(false, "output folder does not exist");
            }

            return ObjectModel.FilePartsValid();
        }

        private void StartRecording()
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

            var part = GetNewPart();

            CumulativeTimeSpan = GetDurationOfExistingParts();
            TimestampHandler.Reset();

            Process.StartInfo.Arguments = $"{Settings.FFMPEGArguments} {GetTargetPartFilename(part)}";
            Process.StartInfo.EnvironmentVariables["FFREPORT"]=$"file={GetTargetPartLogFilename(part)}:level=32";
            Process.StartInfo.WorkingDirectory = ObjectModel.WorkingFolderPath;

            Process.Start();

            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        public Action GetRecordingMethod()
        {
            if (Recording)
            {
                return StopRecording;
            }

            return StartRecording;
        }

        private void StopRecording()
        {
            if (!Recording)
            {
                return;
            }

            Process.StandardInput.WriteLine('q');
        }

        private void ProcessExitHandler(object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => ProcessExitHandler(sender, e));
                return;
            }

            Process.CancelErrorRead();
            Process.CancelOutputRead();
            Recording = false;
        }

        public void ClearExistingParts()
        {
            if (OkToRecord().IsValid == false)
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
            return Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, ObjectModel.ExistingPartsMask);
        }

        private TimeSpan GetDurationOfExistingParts()
        {
            if (OkToRecord().IsValid == false)
            {
                return new TimeSpan();
            }

            var result = new TimeSpan();
            foreach (var part in GetExistingParts())
            {
                var item = ShellObject.FromParsingName(part).Properties.GetProperty<ulong?>("System.Media.Duration");
                if (item == null)
                {
                    // what to do here
                    continue;
                }
                result = result.Add(new TimeSpan(Convert.ToInt64(item.Value)));
            }

            return result;
        }

        public TimeSpan ResetCumulativeTimestamp()
        {
            CumulativeTimeSpan = GetDurationOfExistingParts();
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
            base.Dispose();
        }
    }
}