using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public class CombiningEngine : AbstractEngine
    {
        private const string ArgumentFormat = "-y -f concat -i \"{0}\" -c copy \"{1}\"";
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

        public void Combine()
        {
            if (OkToCombine().IsValid == false)
            {
                return;
            }

            OutputWindowViewModel.StartOutput("Combining");
            GenerateCombineList();
            Combining = true;

            Directory.CreateDirectory(ObjectModel.OutputFolder);
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

        public ValidationResult OkToCombine()
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

        private void GenerateCombineList()
        {
            var builder = new StringBuilder();
            foreach (var file in Directory.EnumerateFiles(ObjectModel.WorkingFolderPath, "*.mkv").OrderBy(f => f))
            {
                builder.AppendLine($"file '{file}'");
            }

            File.WriteAllText(CombineFilePath, builder.ToString());
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
    }
}