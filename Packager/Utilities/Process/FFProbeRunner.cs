using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Utilities.Process
{
    public class FFProbeRunner : IFFProbeRunner
    {
        public FFProbeRunner(IProgramSettings programSettings, IProcessRunner processRunner, IFileProvider fileProvider, IObserverCollection observers)
        {
            ProcessRunner = processRunner;
            FileProvider = fileProvider;
            Observers = observers;
            FFProbePath = programSettings.FFProbePath;
            VideoQualityControlArguments = programSettings.FFProbeVideoQualityControlArguments;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
        }

        private IProcessRunner ProcessRunner { get; }
        private IFileProvider FileProvider { get; }
        private IObserverCollection Observers { get; }
        
        private string BaseProcessingDirectory { get; }

        [ValidateFile]
        public string FFProbePath { get; }

        [Required]
        public string VideoQualityControlArguments { get; }

        public async Task<QualityControlFileModel> GenerateQualityControlFile(ObjectFileModel target)
        {
            var sectionKey = Observers.BeginSection("Generating quality control file: {0}", target.ToFileName());
            try
            {
                var qualityControlFile = new QualityControlFileModel(target);
                var targetDirectory = Path.Combine(BaseProcessingDirectory, target.GetFolderName());

                var targetPath = Path.Combine(targetDirectory, target.ToFileName());
                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException($"{target.ToFileName()} could not be found or is not accessible");
                }

                var xmlPath = Path.Combine(targetDirectory, qualityControlFile.ToIntermediateFileName());
                if (FileProvider.FileExists(xmlPath))
                {
                    throw new FileDirectoryExistsException($"{xmlPath} already exists in processing folder");
                }

                var archivePath = Path.Combine(targetDirectory, qualityControlFile.ToFileName());
                if (FileProvider.FileExists(archivePath))
                {
                    throw new FileDirectoryExistsException($"{archivePath} already exists in processing folder");
                }

                var args = new ArgumentBuilder(string.Format(VideoQualityControlArguments, target.ToFileName()));

                using (var fileOutputBuffer = new FileOutputBuffer(xmlPath, FileProvider))
                {
                    await RunProgram(args, fileOutputBuffer, Path.Combine(BaseProcessingDirectory, target.GetFolderName()));
                } 

                //FileProvider.WriteAllText(xmlPath, xml);
                await FileProvider.ArchiveFile(xmlPath, archivePath);

                Observers.EndSection(sectionKey, $"Quality control file generated successfully: {target.ToFileName()}");
                return qualityControlFile;

            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, target.BarCode);
                throw new LoggedException(e);
            }
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var info = new ProcessStartInfo(FFProbePath) { Arguments = "-version" };
                var result = await ProcessRunner.Run(info);

                var parts = result.StandardOutput.GetContent().Split(' ');
                return parts[2];
            }
            catch (Exception)
            {
                return "";
            }
        }
        
        private async Task RunProgram(IEnumerable arguments, IOutputBuffer outputbuffer, string workingFolder)
        {
            var startInfo = new ProcessStartInfo(FFProbePath)
            {
                Arguments = arguments.ToString(),
                RedirectStandardError = true,
                RedirectStandardOutput = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingFolder
            };

            var result = await ProcessRunner.Run(startInfo, outputbuffer);

            Observers.Log(FilterOutputLog(result.StandardError.GetContent()));

            if (result.ExitCode != 0)
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }

        private static string FilterOutputLog(string log)
        {
            var parts = log.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.StartsWith("[Parsed_cropdetect_2") == false) // remove frame entries
                .ToList();

            // insert empty line before remaining "Parsed" lines
            parts.InsertBefore(p=>p.StartsWith("[Parsed_"), string.Empty);
            // insert empty line before "Input #0" line
            parts.InsertBefore(p => p.StartsWith("Input #0"), string.Empty);
            return string.Join("\n", parts);
        }
    }
}