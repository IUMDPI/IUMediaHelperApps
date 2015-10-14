using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Recorder.Models;

namespace Recorder.Utilities
{
    public class CombiningEngine : AbstractEngine
    {
        private const string ArgumentFormat = "-y -f concat -i \"{0}\" -c copy \"{1}\"";
        private bool _combining;

        public CombiningEngine(IProgramSettings settings, ObjectModel objectModel) : base(settings, objectModel)
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

            GenerateCombineList();
            Combining = true;

            Directory.CreateDirectory(ObjectModel.OutputFolder);
            Process.StartInfo.Arguments = string.Format(ArgumentFormat, CombineFilePath, ObjectModel.OutputFile);

            Process.Start();
        }


        private void ProcessExitHandler(object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => ProcessExitHandler(sender, e));
                return;
            }

            Combining = false;

            OpenFolder();
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

        protected override void ConfigureOutputCapture()
        {
            // nothing to do
        }
    }
}