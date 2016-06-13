using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Utilities.ProcessRunners
{
    public class FFProbeRunner : IFFProbeRunner
    {
        public FFProbeRunner(IProgramSettings programSettings, IProcessRunner processRunner, IFileProvider fileProvider,
            IObserverCollection observers)
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

        public async Task<QualityControlFile> GenerateQualityControlFile(AbstractFile target, CancellationToken cancellationToken)
        {
            var sectionKey = Observers.BeginSection("Generating quality control file: {0}", target.Filename);
            try
            {
                var qualityControlFile = new QualityControlFile(target);
                var targetDirectory = Path.Combine(BaseProcessingDirectory, target.GetFolderName());

                var targetPath = Path.Combine(targetDirectory, target.Filename);
                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException($"{target.Filename} could not be found or is not accessible");
                }

                var xmlPath = Path.Combine(targetDirectory, qualityControlFile.IntermediateFileName);
                if (FileProvider.FileExists(xmlPath))
                {
                    throw new FileDirectoryExistsException($"{xmlPath} already exists in processing folder");
                }

                var archivePath = Path.Combine(targetDirectory, qualityControlFile.Filename);
                if (FileProvider.FileExists(archivePath))
                {
                    throw new FileDirectoryExistsException($"{archivePath} already exists in processing folder");
                }

                var args = new ArgumentBuilder(string.Format(VideoQualityControlArguments, target.Filename));

                using (var fileOutputBuffer = new FileOutputBuffer(xmlPath, FileProvider))
                {
                    await RunProgram(args, fileOutputBuffer, Path.Combine(BaseProcessingDirectory, target.GetFolderName()), 
                        cancellationToken);
                }
                
                await FileProvider.ArchiveFile(xmlPath, archivePath, cancellationToken);

                Observers.EndSection(sectionKey, $"Quality control file generated successfully: {target.Filename}");
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
                var info = new ProcessStartInfo(FFProbePath)
                {
                    Arguments = "-version",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };
                var result = await ProcessRunner.Run(info, CancellationToken.None);

                var parts = result.StandardOutput.GetContent().Split(' ');
                return parts[2];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<InfoFile> GetMediaInfo(AbstractFile target, CancellationToken cancellationToken)
        {
            const string infoArguments = "-print_format xml -show_format -show_streams {0}";

            var sectionKey = Observers.BeginSection("Generating media info: {0}", target.Filename);
            try
            {
                var infoFile = new InfoFile(target);
                var targetDirectory   = Path.Combine(BaseProcessingDirectory, target.GetFolderName());

                var targetPath = Path.Combine(targetDirectory, target.Filename);
                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException($"{target.Filename} could not be found or is not accessible");
                }
                
                var outputPath = Path.Combine(targetDirectory, infoFile.Filename);

                var args = new ArgumentBuilder(string.Format(infoArguments, target.Filename));

                using (var fileOutputBuffer = new FileOutputBuffer(outputPath, FileProvider))
                {
                    await RunProgram(args, fileOutputBuffer, Path.Combine(BaseProcessingDirectory, target.GetFolderName()),
                        cancellationToken);
                }

                Observers.EndSection(sectionKey, $"Media info generated: {target.Filename}");
                return infoFile;

            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, target.BarCode);
                throw new LoggedException(e);
            }

        }

        private async Task RunProgram(IEnumerable arguments, IOutputBuffer outputbuffer, string workingFolder, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo(FFProbePath)
            {
                Arguments = arguments.ToString(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingFolder
            };

            var result = await ProcessRunner.Run(startInfo, cancellationToken, outputbuffer);

            Observers.Log(FilterOutputLog(result.StandardError.GetContent()));

            if (cancellationToken.IsCancellationRequested)
            {
                throw new UserCancelledException();
            }

            if (result.ExitCode != 0)
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }
        
        private static string FilterOutputLog(string log)
        {
            var parts = log.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.StartsWith("[Parsed_cropdetect_2") == false) // remove frame entries
                .ToList();

            // insert empty line before remaining "Parsed" lines
            parts.InsertBefore(p => p.StartsWith("[Parsed_"), string.Empty);
            // insert empty line before "Input #0" line
            parts.InsertBefore(p => p.StartsWith("Input #0"), string.Empty);
            return string.Join("\n", parts);
        }
    }
}