using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Attributes;
using Packager.Exceptions;
using Packager.Extensions;

using Packager.Models;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Utilities
{
    // ReSharper disable once InconsistentNaming
    public class FFMPEGRunner : IFFMPEGRunner
    {
        public FFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider, IHasher hasher)
        {
            FFMPEGPath = programSettings.FFMPEGPath;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
            ProcessRunner = processRunner;
            Observers = observers;
            FileProvider = fileProvider;
            Hasher = hasher;
            AudioAccessArguments = programSettings.FFMPEGAudioAccessArguments;
            BaseProductionArgs = programSettings.FFMPEGAudioProductionArguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private List<string> BaseNormalizingArgs => new List<string>()  { "-map_metadata", "-1" } ;
        private List<string> BaseProductionArgs { get; }
        private string AudioAccessArguments { get; }

        private string ProductionFileExtension => ".wav";

        private string AccessFileExtension => ".mp4";

        private string BaseProcessingDirectory { get; }
        private IProcessRunner ProcessRunner { get; }
        private IObserverCollection Observers { get; }
        private IFileProvider FileProvider { get; }
        private IHasher Hasher { get; }
       
        [ValidateFile]
        public string FFMPEGPath { get; set; }

        public async Task<ObjectFileModel> CreateProductionDerivative(ObjectFileModel original, BextMetadata metadata)
        {
            var target = ToProductionFileModel(original);
            return await CreateDerivative(original, target, GetMetadataArguments(BaseProductionArgs, metadata));
        }

        public async Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            return await CreateDerivative(original, ToAccessFileModel(original), AudioAccessArguments);
        }

        public async Task Verify(List<ObjectFileModel> originals)
        {
            foreach (var model in originals)
            {
                await GenerateMd5Hashes(model);
                await VerifyHashes(model);
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

        private async Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, string arguments)
        {
            var sectionKey = Observers.BeginSection("Generating {0}: {1}", target.FullFileUse, target.ToFileName());
            try
            {
                var outputFolder = Path.Combine(BaseProcessingDirectory, original.GetFolderName());

                var inputPath = Path.Combine(outputFolder, original.ToFileName());
                var outputPath = Path.Combine(outputFolder, target.ToFileName());

                if (FileProvider.FileExists(outputPath))
                {
                    Observers.Log("{0} already exists. Will not generate derivative", target.FullFileUse);
                }
                else
                {
                    var args = $"-i {inputPath} {arguments} {outputPath}";
                    await RunProgram(args);
                }

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
                const string commandLineFormat = "-y -i {0} -f framemd5 {1}";

                var folderPath = Path.Combine(BaseProcessingDirectory,
                    isOriginal ? model.GetOriginalFolderName() : model.GetFolderName());

                var targetPath = Path.Combine(folderPath, model.ToFileName());

                if (FileProvider.FileDoesNotExist(targetPath))
                {
                    throw new FileNotFoundException("Framemd5 target does not exist or is not accessible", targetPath);
                }

                var md5Path = Path.Combine(folderPath, model.ToFrameMd5Filename());

                await RunProgram(string.Format(commandLineFormat, targetPath, md5Path));
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

       /* private async Task Normalize(ObjectFileModel original)
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
                    throw new FileDirectoryExistsException(targetPath);
                }

                var args = $"-i {originalPath.ToQuoted()} -acodec copy -write_bext 1 -rf64 auto {targetPath.ToQuoted()}";
                await RunProgram(args);
                Observers.EndSection(sectionKey, $"{original.ToFileName()} normalized successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }*/

        public async Task Normalize(ObjectFileModel original, BextMetadata core)
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
                    throw new FileDirectoryExistsException(targetPath);
                }

                var args = $"-i {originalPath.ToQuoted()} -acodec copy -write_bext 1 -rf64 auto {GetMetadataArguments(BaseNormalizingArgs, core)} {targetPath.ToQuoted()}";
                Observers.Log(args);
                await RunProgram(args);
                Observers.EndSection(sectionKey, $"{original.ToFileName()} normalized successfully");
            }
            catch (Exception e)
            {
                Observers.LogProcessingIssue(e, original.BarCode);
                Observers.EndSection(sectionKey);
                throw new LoggedException(e);
            }
        }

        private async Task RunProgram(string args)
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


        private string GetMetadataArguments(List<string> args, BextMetadata metadata)
        {
            // normalize the coding history length to ensure that
            // bext chunk comes out with even number of bytes
            // this addresses issue where audio-inspector crashes
            // if bext chunk reports an odd chunk size
            if (!string.IsNullOrWhiteSpace(metadata.CodingHistory) && metadata.CodingHistory.Length%2 == 0)
            {
                metadata.CodingHistory = metadata.CodingHistory + " ";
            }

            foreach (var info in metadata.GetType().GetProperties()
                .Select(p => new Tuple<string, BextFieldAttribute>(GetValueFromField(metadata, p), p.GetCustomAttribute<BextFieldAttribute>()))
                .Where(t => t.Item2 != null && !string.IsNullOrWhiteSpace(t.Item1)))
            {
                if (info.Item2.ValueWithinLengthLimit(info.Item1) == false)
                {
                    throw new BextMetadataException("Value for bext field {0} ('{1}') exceeds maximum length ({2})", info.Item2.Field, info.Item1, info.Item2.MaxLength);
                }

                args.Add($"-metadata {info.Item2.GetFFMPEGArgument()}={info.Item1.NormalizeForCommandLine().ToQuoted()}");
            }

            return string.Join(" ", args);
        }

        private static string GetValueFromField(BextMetadata core, PropertyInfo info)
        {
            return info.GetValue(core).ToDefaultIfEmpty();
        }

        protected ObjectFileModel ToAccessFileModel(ObjectFileModel original)
        {
            return original.ToAccessFileModel(AccessFileExtension);
        }

        protected ObjectFileModel ToProductionFileModel(ObjectFileModel original)
        {
            return original.ToProductionFileModel(ProductionFileExtension);
        }
    }
}