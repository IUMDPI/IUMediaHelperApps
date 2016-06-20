using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public class CombiningEngine : AbstractEngine
    {
        private const string ArgumentFormat = "-y -f concat -i \"{0}\" -c copy -map 0 \"{1}\"";
        private bool _combining;

        public CombiningEngine(IProgramSettings settings, ObjectModel objectModel, OutputWindowViewModel outputModel, IssueNotifyModel issueNotifyModel)
            : base(settings, objectModel, outputModel, issueNotifyModel)
        {
            Process.Exited += ProcessExitHandler;
        }

        public bool Combining
        {
            get { return _combining; }
            set
            {
                _combining = value;
                OnPropertyChanged();
            }
        }


        private string CombineFilePath => Path.Combine(ObjectModel.WorkingFolderPath, "combine.txt");

        public async Task Combine()
        {
            if (OkToProcess().IsValid == false)
            {
                return;
            }

            ResetObservers();

            OutputWindowViewModel.HideVolumeMeter();
            OutputWindowViewModel.StartOutput("Combining");
            Combining = true;

            Directory.CreateDirectory(ObjectModel.OutputFolder);

            var parts= GenerateCombineList();
            
            if (parts.Count == 1)
            {
                await MoveSinglePart(parts.First());
            }
            else
            {
                CombineWithFFMpeg();
            }
        }

        private async Task MoveSinglePart(string part)
        {
            OutputWindowViewModel.WriteLine("Only 1 part exists");
            OutputWindowViewModel.WriteLine($"Renaming to {ObjectModel.Filename} and moving to output folder");
            if (File.Exists(ObjectModel.OutputFile))
            {
                OutputWindowViewModel.WriteLine($"{ObjectModel.Filename} already exists");
                OutputWindowViewModel.WriteLine($"Removing {ObjectModel.Filename} (existing version)");
                await Task.Run(() => { File.Delete(ObjectModel.OutputFile); });
            }

            OutputWindowViewModel.WriteLine("");
            OutputWindowViewModel.WriteLine($"Moving {Path.GetFileName(part)} to {ObjectModel.Filename}");

            await Task.Run(() => { File.Move(part, ObjectModel.OutputFile); });

            OutputWindowViewModel.WriteLine("");
            OutputWindowViewModel.WriteLine("File moved successfully!");
            OpenFolder();
            Combining = false;
        }


        private void CombineWithFFMpeg()
        {
            Process.StartInfo.Arguments = string.Format(ArgumentFormat, CombineFilePath, ObjectModel.OutputFile);
            Process.StartInfo.EnvironmentVariables["FFREPORT"] = $"file={ObjectModel.ProjectCode}_{ ObjectModel.Barcode}.log:level=32";
            Process.StartInfo.WorkingDirectory = ObjectModel.OutputFolder;

            Process.Start();

            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();

        }

        private void ProcessExitHandler(object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => ProcessExitHandler(sender, e));
                return;
            }

            try
            {
                Combining = false;
                CheckExitCode();
            }
            finally
            {
                Process.CancelOutputRead();
                Process.CancelErrorRead();
            }
        }


        private void OpenFolder()
        {
            var info = new ProcessStartInfo(ObjectModel.OutputFolder);
            using (var process = new Process {StartInfo = info})
            {
                process.Start();
            }
        }


        private List<string> GenerateCombineList()
        {
            var builder = new StringBuilder();
            var parts = Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, "*.mkv").OrderBy(f => f).ToList();
            foreach (var file in parts)
            {
                builder.AppendLine($"file '{file}'");
            }

            File.WriteAllText(CombineFilePath, builder.ToString());
            return parts;
        }

        protected override void CheckExitCode()
        {
            if (Process.ExitCode == 0)
            {
                OpenFolder();
                return;
            }

            IssueNotifyModel.Notify(GetErrorMessageFromOutput("Something went wrong while combining parts: {0}"));
        }

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

        public override void LogConfiguration(OutputWindowViewModel outputModel)
        {
            var globalConfigOk = IsGlobalConfigurationOk();
            if (globalConfigOk.IsValid)
            {
                outputModel.WriteLine("Combiner: initialized");
            }
            else
            {
                outputModel.WriteLine("Combiner: not initialized");
                outputModel.WriteLine($"Combiner: {globalConfigOk.ErrorContent}");
                outputModel.WriteLine("");
            }
        }
    }
}