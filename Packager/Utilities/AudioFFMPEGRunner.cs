using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;

namespace Packager.Utilities
{
    // ReSharper disable once InconsistentNaming
    public class AudioFFMPEGRunner : AbstractFFMPEGRunner, IAudioFFMPEGRunner
    {
        private const string NormalizingArguments = "-acodec copy -write_bext 1 -rf64 auto -map_metadata -1";
        private const string RequiredProductionBextArguments = "-write_bext 1";
        private const string RequiredProductionRiffArguments = "-rf64 auto";

        public AudioFFMPEGRunner(IProgramSettings programSettings, IProcessRunner processRunner, IObserverCollection observers, IFileProvider fileProvider, IHasher hasher) :
            base(programSettings, processRunner, observers, fileProvider)
        {
            Hasher = hasher;
            AccessArguments = programSettings.FFMPEGAudioAccessArguments;
            ProductionArguments = AddRequiredArguments(programSettings.FFMPEGAudioProductionArguments);
        }
        
        private IHasher Hasher { get; }

        public string ProductionArguments { get; }
        public string AccessArguments { get; }


        public async Task<ObjectFileModel> CreateProductionDerivative(ObjectFileModel original, ObjectFileModel target, EmbeddedAudioMetadata metadata)
        {
            if (TargetAlreadyExists(target))
            {
                LogAlreadyExists(target);
                return target;
            }

            var args = new ArgumentBuilder(ProductionArguments)
                .AddArguments(metadata.AsArguments());

            return await CreateDerivative(original, target, args);
        }

        public async Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original)
        {
            return await CreateDerivative(original, original.ToAudioAccessFileModel(), new ArgumentBuilder(AccessArguments));
        }

        public async Task Verify(List<ObjectFileModel> originals)
        {
            foreach (var model in originals)
            {
                await GenerateMd5Hashes(model);
                await VerifyHashes(model);
            }
        }

        public async Task Normalize(ObjectFileModel original, EmbeddedAudioMetadata metadata)
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

        private static string AddRequiredArguments(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return arguments;
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionBextArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionBextArguments}";
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionRiffArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionRiffArguments}";
            }

            return arguments;
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

                var arguments = new ArgumentBuilder(string.Format(commandLineFormat, targetPath, md5Path));
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