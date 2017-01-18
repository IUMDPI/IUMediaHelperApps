using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Providers;
using Packager.Utilities.Hashing;

namespace Packager.Utilities.Images
{
    public class LabelImageImporter : ILabelImageImporter
    {
        private IFileProvider FileProvider { get; }
        private IDirectoryProvider DirectoryProvider { get; }
        private IHasher Hasher { get; }
        private string ImageDirectory { get; }
        private string ProjectCode { get; }
        private string BaseProcessingFolder { get; }
       
        public LabelImageImporter(IProgramSettings settings, IFileProvider fileProvider, IDirectoryProvider directoryProvider, IHasher hasher)
        {
            ProjectCode = settings.ProjectCode;
            ImageDirectory = settings.ImageDirectory;
            BaseProcessingFolder = settings.ProcessingDirectory;

            FileProvider = fileProvider;
            DirectoryProvider = directoryProvider;
            Hasher = hasher;
        }

        public async Task<List<AbstractFile>> ImportMediaImages(string barcode, CancellationToken cancellationToken)
        {
            // 1 find images
            // 2 copy images to processing directory
            // return list of models

            // does folder exist?
            var sourceFolder = Path.Combine(ImageDirectory, $"{ProjectCode}_{barcode}");
            if (DirectoryProvider.DirectoryExists(sourceFolder) == false)
            {
                // if not return empty list
                return new List<AbstractFile>();
            }

            // get .tiff file models
            var imageFiles = DirectoryProvider.EnumerateFiles(sourceFolder)
                    .Select(FileModelFactory.GetModel)
                    .Where(f => f is TiffImageFile)
                    .Where(f=>f.IsValid())
                    .ToList();
            
            // copy files to processing folder
            foreach (var file in imageFiles)
            {
                var sourcePath = Path.Combine(sourceFolder, file.Filename);
                var destPath = Path.Combine(BaseProcessingFolder, file.GetFolderName(), file.Filename);

                var originalHash = await Hasher.Hash(sourcePath, cancellationToken);
                await FileProvider.CopyFileAsync(sourcePath, destPath, cancellationToken);
                var destHash = await Hasher.Hash(destPath, cancellationToken);

                if (originalHash != destHash)
                {
                    throw new Exception($"copy hash ({destHash}) is not equal to original hash ({originalHash}).");
                }

            }

            return imageFiles;
        }
    }
}
