using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Hashing;
using Packager.Validators.Attributes;

namespace Packager.Utilities.ProcessRunners
{
    public abstract class AbstractFFMPEGRunner : IFFMPEGRunner
    {
        private readonly CancellationToken _cancellationToken;

        protected AbstractFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner,
            IObserverCollection observers, IFileProvider fileProvider, IHasher hasher, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            FFMPEGPath = programSettings.FFMPEGPath;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
            Observers = observers;
            FileProvider = fileProvider;
            Hasher = hasher;
            ProcessRunner = processRunner;
        }

        protected abstract string NormalizingArguments { get; }
        private IProcessRunner ProcessRunner { get; }
        private IObserverCollection Observers { get; }
        private IFileProvider FileProvider { get; }
        private IHasher Hasher { get; }
        private string BaseProcessingDirectory { get; }

        [ValidateFile]
        public string FFMPEGPath { get; set; }

        public async Task<string> GetFFMPEGVersion()
        {
            try
            {
                var info = new ProcessStartInfo(FFMPEGPath)
                {
                    Arguments = "-version",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };
                var result = await ProcessRunner.Run(info);

                var parts = result.StandardOutput.GetContent().Split(' ');
                return parts[2];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task Normalize(AbstractFile original, AbstractEmbeddedMetadata metadata)
        {
            var sectionKey = Observers.BeginSection("Normalizing {0}", original.Filename);
            try
            {
                var originalPath = Path.Combine(BaseProcessingDirectory, original.GetOriginalFolderName(),
                    original.Filename);
                if (FileProvider.FileDoesNotExist(originalPath))
                {
                    throw new FileNotFoundException("Original does not exist or is not accessible", originalPath);
                }

                var targetPath = Path.Combine(BaseProcessingDirectory, original.GetFolderName(), original.Filename);
                if (FileProvider.FileExists(targetPath))
                {
                    throw new FileDirectoryExistsException("{0} already exists in the processing directory", targetPath);
                }

                var arguments = new ArgumentBuilder($"-i {originalPath.ToQuoted()}")
                    .AddArguments(NormalizingArguments)
                    .AddArguments(metadata.AsArguments())
                    .AddArguments(targetPath.ToQuoted());

                await RunProgram(arguments);
                Observers.EndSection(sectionKey, $"{original.Filename} normalized successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        public async Task Verify(List<AbstractFile> originals)
        {
            foreach (var model in originals)
            {
                await GenerateMd5Hashes(model);
                await VerifyHashes(model);
            }
        }

        public abstract string ProdOrMezzArguments { get; }
        public abstract string AccessArguments { get; }

        public async Task<AbstractFile> CreateAccessDerivative(AbstractFile original)
        {
            return await CreateDerivative(original, new AccessFile(original), new ArgumentBuilder(AccessArguments));
        }

        public async Task<AbstractFile> CreateProdOrMezzDerivative(AbstractFile original, AbstractFile target,
            AbstractEmbeddedMetadata metadata)
        {
            if (TargetAlreadyExists(target))
            {
                LogAlreadyExists(target);
                return target;
            }

            var args = new ArgumentBuilder(ProdOrMezzArguments)
                .AddArguments(metadata.AsArguments());

            return await CreateDerivative(original, target, args);
        }

        private async Task RunProgram(IEnumerable arguments)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = arguments.ToString(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.RunWithCancellation(startInfo, _cancellationToken);

            Observers.Log(FilterOutputLog(result.StandardError.GetContent()));

            if (_cancellationToken.IsCancellationRequested)
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
                .Where(p => p.StartsWith("frame=") == false) // remove frame entries
                .Where(p => p.StartsWith("size=") == false) // remove size entries
                .Where(p => p.StartsWith("Press [q] to stop") == false) // remove press q message
                .ToList();

            // insert empty line before "Input #0" line
            parts.InsertBefore(p => p.StartsWith("Input #0"), string.Empty);
            // insert empty line before "Output #0" line
            parts.InsertBefore(p => p.StartsWith("Output #0"), string.Empty);
            // insert empty line before "Stream mapping:" line
            parts.InsertBefore(p => p.StartsWith("Stream mapping:"), string.Empty);
            // insert empty line before "video summary" line
            parts.InsertBefore(p => p.StartsWith("video:"), string.Empty);
            // insert empty line before "guesed" lines
            parts.InsertBefore(p => p.StartsWith("Guessed"), string.Empty);
            // insert empty line before "[matroska @...] lines
            parts.InsertBefore(p => p.StartsWith("[matroska @"));
            // insert empty line before "[libx264 @...] lines
            parts.InsertBefore(p => p.StartsWith("[libx264 @"));

            return string.Join("\n", parts);
        }

        private void LogAlreadyExists(AbstractFile target)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.Filename);
            Observers.Log("{0} already exists. Will not generate derivative", target.FullFileUse);
            Observers.EndSection(sectionKey,
                $"Generate {target.FullFileUse} skipped - already exists: {target.Filename}");
        }

        private bool TargetAlreadyExists(AbstractFile target)
        {
            var path = Path.Combine(BaseProcessingDirectory, target.GetFolderName(), target.Filename);
            return FileProvider.FileExists(path);
        }

        private async Task<AbstractFile> CreateDerivative(AbstractFile original, AbstractFile target,
            ArgumentBuilder arguments)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.Filename);
            try
            {
                var outputFolder = Path.Combine(BaseProcessingDirectory, original.GetFolderName());

                var inputPath = Path.Combine(outputFolder, original.Filename);
                if (FileProvider.FileDoesNotExist(inputPath))
                {
                    throw new FileNotFoundException(inputPath);
                }

                var outputPath = Path.Combine(outputFolder, target.Filename);
                if (FileProvider.FileExists(outputPath))
                {
                    throw new FileDirectoryExistsException("{0} already exists in the processing directory", inputPath);
                }

                var completeArguments = new ArgumentBuilder($"-i {inputPath.ToQuoted()}")
                    .AddArguments(arguments)
                    .AddArguments(outputPath.ToQuoted());

                await RunProgram(completeArguments);

                Observers.EndSection(sectionKey, $"{target.FullFileUse} generated successfully: {target.Filename}");
                return target;
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task GenerateMd5Hashes(AbstractFile original)
        {
            await GenerateMd5Hash(original, true);
            await GenerateMd5Hash(original, false);
        }

        private async Task GenerateMd5Hash(AbstractFile model, bool isOriginal)
        {
            var sectionName = isOriginal
                ? $"{model.Filename} (original)"
                : $"{model.Filename} (normalized)";
            var sectionKey = Observers.BeginSection("Hashing {0}", sectionName);
            try
            {
                var folderPath = Path.Combine(BaseProcessingDirectory,
                    isOriginal ? model.GetOriginalFolderName() : model.GetFolderName());

                var targetPath = Path.Combine(folderPath, model.Filename);

                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException("Framemd5 target does not exist or is not accessible", targetPath);
                }

                var md5Path = Path.Combine(folderPath, model.ToFrameMd5Filename());

                var arguments = new ArgumentBuilder($"-y -i {targetPath}")
                    .AddArguments($"-f framemd5 {md5Path}");

                await RunProgram(arguments);
                Observers.EndSection(sectionKey, $"{sectionName} hashed successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, model.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task VerifyHashes(AbstractFile model)
        {
            var sectionKey = Observers.BeginSection("Validating {0} (normalized)", model.Filename);
            try
            {
                var originalFrameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(),
                    model.ToFrameMd5Filename());
                if (FileProvider.FileDoesNotExist(originalFrameMd5Path))
                {
                    throw new FileNotFoundException("framemd5 file does not exist or is not accessible",
                        originalFrameMd5Path);
                }

                var originalMd5Hash = await Hasher.Hash(originalFrameMd5Path, _cancellationToken);
                Observers.Log("original framemd5 hash: {0}", originalMd5Hash);

                var normalizedFrameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetFolderName(),
                    model.ToFrameMd5Filename());
                if (FileProvider.FileDoesNotExist(normalizedFrameMd5Path))
                {
                    throw new FileNotFoundException("framemd5 fil does not exist or is not accessible",
                        normalizedFrameMd5Path);
                }

                var normalizedMd5Hash = await Hasher.Hash(normalizedFrameMd5Path, _cancellationToken);
                Observers.Log("normalized framemd5 hash: {0}", normalizedMd5Hash);

                if (!originalMd5Hash.Equals(normalizedMd5Hash))
                {
                    throw new NormalizeOriginalException("Original and normalized framemd5 hashes not equal: {0} {1}",
                        originalMd5Hash, normalizedMd5Hash);
                }

                Observers.EndSection(sectionKey, $"{model.Filename} (normalized) validated successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, model.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }
    }
}