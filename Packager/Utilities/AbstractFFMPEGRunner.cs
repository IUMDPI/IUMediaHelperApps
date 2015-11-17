using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Models;
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
    }
}