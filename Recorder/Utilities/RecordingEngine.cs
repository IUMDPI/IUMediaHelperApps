using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;
using Recorder.Models;
using Recorder.Receivers;

namespace Recorder.Utilities
{
    public class RecordingEngine : AbstractEngine
    {
        public RecordingEngine(ProgramSettings settings, ObjectModel objectModel) : base(settings, objectModel)
        {
            Recording = false;
        }

        public void InitializeTimestampDelegate(OutputReceivedHandler.TimestampDelegate @delegate)
        {
            Process.ErrorDataReceived += new OutputReceivedHandler(@delegate).OnDataReceived;
            Process.OutputDataReceived += new OutputReceivedHandler(@delegate).OnDataReceived;
        }
        
        public bool Recording { get; private set; }

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

            Process.StartInfo.Arguments = $"{Settings.FFMPEGArguments} -vstats_file {GetTargetVstatFileName(part)} {GetTargetPartFilename(part)}";
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
            Process.CancelErrorRead();
            Process.CancelOutputRead();           
            Recording = false;
        }

        public void ClearExistingParts()
        {
            foreach (var file in Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, "*.mkv"))
            {
                FileSystem.DeleteFile(file, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }

            foreach (var file in Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, "*.log"))
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


        private string GetTargetPartFilename(int part)
        {
            return  $"{ObjectModel.ProjectCode}_{ObjectModel.Barcode}_part_{part:d5}.mkv";
        }

        private string GetTargetVstatFileName(int part)
        {
            return $"{ObjectModel.ProjectCode}_{ObjectModel.Barcode}_vstat_{part:d5}.log";
        }
        
    }


}