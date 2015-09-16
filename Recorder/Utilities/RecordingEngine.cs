using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using Recorder.Models;

namespace Recorder.Utilities
{
    public class RecordingEngine:IDisposable
    {
        private readonly Process _process;

        private readonly Regex _barcodeExpression = new Regex(@"^\d{14,14}$");

        public RecordingEngine(ProgramSettings settings)
        {
            Settings = settings;
            Recording = false;

            FileUses = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Preservation", "pres"),
                new Tuple<string, string>("Preservation-Intermediate", "pres-int"),
                new Tuple<string, string>("Production", "prod")
            };

            _process = new Process
            {
                StartInfo = new ProcessStartInfo(Settings.PathToFFMPEG)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true
                }
            };

        }

        private ProgramSettings Settings { get; }

        public List<Tuple<string, string>> FileUses { get; private set; }

        public bool Recording { get; private set; }
        public string Barcode { get; set; }
        public int Part { get; set; }

        public string ProjectCode => Settings.ProjectCode;

        public string FileUse { get; set; }

        public string Filename => FilePartsValid().IsValid
            ? string.Format($"{ProjectCode}_{Barcode}_{Part:d2}_{FileUse}.mkv")
            : string.Empty;

        public ValidationResult FilePartsValid()
        {
            if (BarcodeValid() == false)
            {
                return new ValidationResult(false, "invalid barcode");
            }

            if (PartValid() == false)
            {
                return new ValidationResult(false, "invalid part");
            }

            if (FileUseValid() == false)
            {
                return new ValidationResult(false, "invaid file use");
            }

            return new ValidationResult(true, "");
        }

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

            return FilePartsValid();
        }

        private bool BarcodeValid()
        {
            return !string.IsNullOrWhiteSpace(Barcode) && _barcodeExpression.IsMatch(Barcode);
        }

        private bool PartValid()
        {
            return Part >= 1 && Part <= 5;
        }

        private bool FileUseValid()
        {
            return string.IsNullOrWhiteSpace(FileUse) == false;
        }

        private void StartRecording()
        {
            if (Recording)
            {
                return;
            }

            Recording = true;

            if (!Directory.Exists(WorkingFolderPath))
            {
                Directory.CreateDirectory(WorkingFolderPath);
            }

            _process.StartInfo.Arguments = $"{Settings.FFMPEGArguments} \"{GetNewPart()}\"";
                
            _process.Start();
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

            _process.StandardInput.WriteLine('q');
            Recording = false;
        }

        private string WorkingFolderName => $"{ProjectCode}_{Barcode}";

        private string WorkingFolderPath => Path.Combine(Settings.WorkingFolder, WorkingFolderName);

        private string GetNewPart()
        {
            var count = 0;
            var value = Path.Combine(WorkingFolderPath, $"part_{count}.mkv");
            while (File.Exists(value))
            {
                count++;
                value = Path.Combine(WorkingFolderPath, $"part_{count}.mkv");
                Thread.Sleep(5);
            }

            return value;
        }

        public bool PartsPresent()
        {
            if (!Directory.Exists(WorkingFolderPath))
            {
                return false;
            }

            return Directory.EnumerateFiles(WorkingFolderPath, "*.mkv").Any();
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}