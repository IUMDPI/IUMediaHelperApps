using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Utilities
{
    public abstract class AbstractFFMPEGRunner : IFFMPEGRunner
    {
        protected AbstractFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider)
        {
            FFMPEGPath = programSettings.FFMPEGPath;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
            Observers = observers;
            FileProvider = fileProvider;
            ProcessRunner = processRunner;
        }

        protected IProcessRunner ProcessRunner { get; }

        protected IObserverCollection Observers { get; }
        protected IFileProvider FileProvider { get; }
        protected string BaseProcessingDirectory { get; }

        [ValidateFile]
        public string FFMPEGPath { get; set; }

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

        protected async Task RunProgram(IEnumerable arguments)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = arguments.ToString(),
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

        protected void LogAlreadyExists(ObjectFileModel target)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.ToFileName());
            Observers.Log("{0} already exists. Will not generate derivative", target.FullFileUse);
            Observers.EndSection(sectionKey, $"Generate {target.FullFileUse} skipped - already exists: {target.ToFileName()}");
        }

        protected bool TargetAlreadyExists(ObjectFileModel target)
        {
            var path = Path.Combine(BaseProcessingDirectory, target.GetFolderName(), target.ToFileName());
            return FileProvider.FileExists(path);
        }

        protected async Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, ArgumentBuilder arguments)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.ToFileName());
            try
            {
                var outputFolder = Path.Combine(BaseProcessingDirectory, original.GetFolderName());

                var inputPath = Path.Combine(outputFolder, original.ToFileName());
                if (FileProvider.FileDoesNotExist(inputPath))
                {
                    throw new FileNotFoundException(inputPath);
                }

                var outputPath = Path.Combine(outputFolder, target.ToFileName());
                if (FileProvider.FileExists(outputPath))
                {
                    throw new FileDirectoryExistsException("{0} already exists in the processing directory", inputPath);
                }

                var completeArguments = new ArgumentBuilder($"-i {inputPath}")
                    .AddArguments(arguments)
                    .AddArguments(outputPath);

                await RunProgram(completeArguments);


                Observers.EndSection(sectionKey, $"{target.FullFileUse} generated successfully: {target.ToFileName()}");
                return target;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }
    }
}