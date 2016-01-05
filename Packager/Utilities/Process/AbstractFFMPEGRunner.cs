using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities.Hashing;
using Packager.Validators.Attributes;

namespace Packager.Utilities.Process
{
    public abstract class AbstractFFMPEGRunner : IFFMPEGRunner
    {
        protected AbstractFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider, IHasher hasher)
        {
            FFMPEGPath = programSettings.FFMPEGPath;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
            Observers = observers;
            FileProvider = fileProvider;
            Hasher = hasher;
            ProcessRunner = processRunner;
            LogLevel = programSettings.FFMPEGLogLevel;
        }

        protected string LogLevel { get; }

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
                var info = new ProcessStartInfo(FFMPEGPath) {Arguments = "-version"};
                var result = await ProcessRunner.Run(info);

                var parts = result.StandardOutput.GetContent().Split(' ');
                return parts[2];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task Normalize(ObjectFileModel original, AbstractEmbeddedMetadata metadata)
        {
            var sectionKey = Observers.BeginSection("Normalizing {0}", original.ToFileName());
            try
            {
                var originalPath = Path.Combine(BaseProcessingDirectory, original.GetOriginalFolderName(), original.ToFileName());
                if (FileProvider.FileDoesNotExist(originalPath))
                {
                    throw new FileNotFoundException("Original does not exist or is not accessible", originalPath);
                }

                var targetPath = Path.Combine(BaseProcessingDirectory, original.GetFolderName(), original.ToFileName());
                if (FileProvider.FileExists(targetPath))
                {
                    throw new FileDirectoryExistsException("{0} already exists in the processing directory", targetPath);
                }

                var arguments = new ArgumentBuilder($"-i {originalPath.ToQuoted()}")
                    .AddArguments(GetLogLevelArguments())
                    .AddArguments(NormalizingArguments)
                    .AddArguments(metadata.AsArguments())
                    .AddArguments(targetPath.ToQuoted());

                await RunProgram(arguments);
                Observers.EndSection(sectionKey, $"{original.ToFileName()} normalized successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private string GetLogLevelArguments()
        {
            return string.IsNullOrWhiteSpace(LogLevel) 
                ? "" 
                : $"-loglevel {LogLevel}";
        }

        public async Task Verify(List<ObjectFileModel> originals)
        {
            foreach (var model in originals)
            {
                await GenerateMd5Hashes(model);
                await VerifyHashes(model);
            }
        }

        public abstract string ProdOrMezzArguments { get; }
        public abstract string AccessArguments { get; }

        public async Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            return await CreateDerivative(original, original.ToAudioAccessFileModel(), new ArgumentBuilder(AccessArguments));
        }

        public async Task<ObjectFileModel> CreateProdOrMezzDerivative(ObjectFileModel original, ObjectFileModel target, AbstractEmbeddedMetadata metadata)
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
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run(startInfo);

            Observers.Log(FilterOutputLog(result.StandardError.GetContent()));

            if (result.ExitCode != 0)
            {
                throw new GenerateDerivativeException("Could not generate derivative: {0}", result.ExitCode);
            }
        }

        private static string FilterOutputLog(string log)
        {
            var parts = log.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(p=>p.StartsWith("frame=")==false) // remove frame entries
                .Where(p=>p.StartsWith("Press [q] to stop")==false) // remove press q message
                .ToList();

            // insert empty line before "Input #0" line
            parts.InsertBefore(p=>p.StartsWith("Input #0"), string.Empty);
            // insert empty line before "Output #0" line
            parts.InsertBefore(p => p.StartsWith("Output #0"), string.Empty);
            // insert empty line before "Stream mapping:" line
            parts.InsertBefore(p => p.StartsWith("Stream mapping:"), string.Empty);
            // insert empty line before "video summary" line
            parts.InsertBefore(p => p.StartsWith("video:"), string.Empty);
            // insert empty line before "guesed" lines
            parts.InsertBefore(p => p.StartsWith("Guessed"), string.Empty);
            // insert empty line before "[matroska @...] lines
            parts.InsertBefore(p=>p.StartsWith("[matroska @"));
            // insert empty line before "[libx264 @...] lines
            parts.InsertBefore(p => p.StartsWith("[libx264 @"));

            return string.Join("\n", parts);
        }

        

        private void LogAlreadyExists(ObjectFileModel target)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.ToFileName());
            Observers.Log("{0} already exists. Will not generate derivative", target.FullFileUse);
            Observers.EndSection(sectionKey, $"Generate {target.FullFileUse} skipped - already exists: {target.ToFileName()}");
        }

        private bool TargetAlreadyExists(ObjectFileModel target)
        {
            var path = Path.Combine(BaseProcessingDirectory, target.GetFolderName(), target.ToFileName());
            return FileProvider.FileExists(path);
        }

        private async Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, ArgumentBuilder arguments)
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

                var completeArguments = new ArgumentBuilder($"-i {inputPath.ToQuoted()}")
                    .AddArguments(GetLogLevelArguments())
                    .AddArguments(arguments)
                    .AddArguments(outputPath.ToQuoted());

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

        private async Task GenerateMd5Hashes(ObjectFileModel original)
        {
            await GenerateMd5Hash(original, true);
            await GenerateMd5Hash(original, false);
        }

        private async Task GenerateMd5Hash(ObjectFileModel model, bool isOriginal)
        {
            var sectionName = isOriginal
                ? $"{model.ToFileName()} (original)"
                : $"{model.ToFileName()} (normalized)";
            var sectionKey = Observers.BeginSection("Hashing {0}", sectionName);
            try
            {
                var folderPath = Path.Combine(BaseProcessingDirectory,
                    isOriginal ? model.GetOriginalFolderName() : model.GetFolderName());

                var targetPath = Path.Combine(folderPath, model.ToFileName());

                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException("Framemd5 target does not exist or is not accessible", targetPath);
                }

                var md5Path = Path.Combine(folderPath, model.ToFrameMd5Filename());

                var arguments = new ArgumentBuilder($"-y -i {targetPath}")
                    .AddArguments(GetLogLevelArguments())
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

        private async Task VerifyHashes(ObjectFileModel model)
        {
            var sectionKey = Observers.BeginSection("Validating {0} (normalized)", model.ToFileName());
            try
            {
                var originalFrameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFrameMd5Filename());
                if (FileProvider.FileDoesNotExist(originalFrameMd5Path))
                {
                    throw new FileNotFoundException("framemd5 file does not exist or is not accessible", originalFrameMd5Path);
                }

                var originalMd5Hash = await Hasher.Hash(originalFrameMd5Path);
                Observers.Log("original framemd5 hash: {0}", originalMd5Hash);

                var normalizedFrameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFrameMd5Filename());
                if (FileProvider.FileDoesNotExist(normalizedFrameMd5Path))
                {
                    throw new FileNotFoundException("framemd5 fil does not exist or is not accessible", normalizedFrameMd5Path);
                }

                var normalizedMd5Hash = await Hasher.Hash(normalizedFrameMd5Path);
                Observers.Log("normalized framemd5 hash: {0}", normalizedMd5Hash);

                if (!originalMd5Hash.Equals(normalizedMd5Hash))
                {
                    throw new NormalizeOriginalException("Original and normalized framemd5 hashes not equal: {0} {1}", originalMd5Hash, normalizedMd5Hash);
                }

                Observers.EndSection(sectionKey, $"{model.ToFileName()} (normalized) validated successfully");
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