using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators.Attributes;
using Packager.Verifiers;

namespace Packager.Utilities
{
    // ReSharper disable once InconsistentNaming
    public class FFMPEGRunner : IFFMPEGRunner
    {
        public FFMPEGRunner(string ffmpegPath, string baseProcessingDirectory, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider)
        {
            FFMPEGPath = ffmpegPath;
            BaseProcessingDirectory = baseProcessingDirectory;
            ProcessRunner = processRunner;
            Observers = observers;
            FileProvider = fileProvider;
        }

        public string BaseProcessingDirectory { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IObserverCollection Observers { get; set; }
        public IFileProvider FileProvider { get; set; }

        [ValidateFile]
        public string FFMPEGPath { get; set; }

        public async Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, string arguments, string outputFolder)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.ToFileName());
            try
            {
                var inputPath = Path.Combine(outputFolder, original.ToFileName());
                var outputPath = Path.Combine(outputFolder, target.ToFileName());

                if (FileProvider.FileExists(outputPath))
                {
                    Observers.Log("{0} already exists. Will not generate derivative", target.FullFileUse);
                }
                else
                {
                    var args = string.Format("-i {0} {1} {2}", inputPath, arguments, outputPath);
                    await CreateDerivative(args);
                }

                Observers.EndSection(sectionKey, string.Format("{0} generated successfully: {1}", target.FullFileUse, target.ToFileName()));
                return target;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }
        
        public async Task<string> GetFFMPEGVersion()
        {
            try
            {
                var info = new ProcessStartInfo(FFMPEGPath) {Arguments = "-version"};
                var result = await ProcessRunner.Run(info);

                var parts = result.StandardOutput.Split(' ');
                return parts[2];
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task CreateDerivative(string args)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run(startInfo);

            Observers.Log(result.StandardError);

            if (result.ExitCode != 0)
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }
    }
}