using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Providers;
using Packager.Utilities.Hashing;
using Packager.Utilities.Xml;
using File = Packager.Models.OutputModels.File;
using StringExtensions = Common.Extensions.StringExtensions;

namespace Packager.Utilities.Images
{
    public class LabelImageImporter : ILabelImageImporter
    {
        private IFileProvider FileProvider { get; }
        private IDirectoryProvider DirectoryProvider { get; }
        private IHasher Hasher { get; }
        private IXmlExporter XmlExporter { get; }

        private string ImageDirectory { get; }
        private string ProjectCode { get; }
        private string BaseProcessingFolder { get; }
       
        public LabelImageImporter(IProgramSettings settings, IFileProvider fileProvider, IDirectoryProvider directoryProvider, IHasher hasher, IXmlExporter xmlExporter)
        {
            ProjectCode = settings.ProjectCode;
            ImageDirectory = settings.ImageDirectory;
            BaseProcessingFolder = settings.ProcessingDirectory;

            FileProvider = fileProvider;
            DirectoryProvider = directoryProvider;
            Hasher = hasher;
            XmlExporter = xmlExporter;
        }

        public async Task<List<AbstractFile>> ImportMediaImages(string barcode, CancellationToken cancellationToken)
        {
            // 1 find image manifest file
            // 2 use manifest to identify images
            // 3 copy images to processing directory
            // 4 verify hashes
            // 5 return list of models

            // does folder exist?
            var sourceFolder = Path.Combine(ImageDirectory, $"{barcode}");
            if (DirectoryProvider.DirectoryExists(sourceFolder) == false)
            {
                // if not return empty list
                return new List<AbstractFile>();
            }

            var destFolder = Path.Combine(BaseProcessingFolder, $"{ProjectCode}_{barcode}");
            if (DirectoryProvider.DirectoryExists(destFolder) == false)
            {
                throw new DirectoryNotFoundException($"Invalid processing folder: {destFolder}");
            }

            var manifestPath = GetManifestPath(barcode, sourceFolder);
            if (FileProvider.FileDoesNotExist(manifestPath))
            {
                throw new FileNotFoundException("Label image manifest file not present", manifestPath);
            }
            
            var labelFiles = GetImageFiles(manifestPath);

            // copy files to processing folder and verify each file
            foreach (var file in labelFiles)
            {
                var sourcePath = Path.Combine(sourceFolder, file.Filename);
                var destPath = Path.Combine(destFolder, file.Filename);

                await FileProvider.CopyFileAsync(sourcePath, destPath, cancellationToken);
                await VerifyFile(file, destPath, cancellationToken);
            }

            return labelFiles;
        }

        public bool LabelImagesPresent(AbstractPodMetadata metadata)
        {
            var sourceFolder = Path.Combine(ImageDirectory, $"{metadata.Barcode}");
            if (DirectoryProvider.DirectoryExists(sourceFolder) == false)
            {
                return false;
            }

            var manifestPath = GetManifestPath(metadata.Barcode, sourceFolder);
            if (FileProvider.FileDoesNotExist(manifestPath))
            {
                return false;
            }

            var labels = GetImageFiles(manifestPath).Select(f=>f.SequenceIndicator).OrderBy(v=>v);

            var expected = metadata.FileProvenances
                .Select(f => FileModelFactory.GetModel(f.Filename))
                .Where(f => f.IsPreservationVersion()).Select(f=>f.SequenceIndicator)
                .OrderBy(v=>v);

            return labels.SequenceEqual(expected);

        }

        private string GetManifestPath(string barcode, string sourceFolder)
        {
            return Path.Combine(sourceFolder, $"{ProjectCode}_{barcode}.xml");
        }

        private List<AbstractFile> GetImageFiles(string manifestPath)
        {
            // import the manifest
            var manifest = XmlExporter.ImportFromFile<ImportableManifest<RecordCarrier>>(manifestPath);
            var fileEntries = manifest.Carrier.Parts.Sides.SelectMany(s => s.Files);
            // get .tiff file models
            return fileEntries
                .Select(entry => CreateFileAndAssignHash(entry, Path.GetDirectoryName(manifestPath)))
                .Where(f => f is TiffImageFile)
                .Where(f => f.IsImportable())
                .ToList();
        }

        private async Task VerifyFile(AbstractFile file, string destPath, CancellationToken cancellationToken)
        {
            var destChecksum = await Hasher.Hash(destPath, cancellationToken);
            if (string.Equals(file.Checksum, destChecksum, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new Exception($"copy hash ({destChecksum.ToUpperInvariant()}) is not equal to original hash ({file.Checksum.ToUpperInvariant()}).");
            }
        }

        private static AbstractFile CreateFileAndAssignHash(File entry, string sourceFolder)
        {
            var result = FileModelFactory.GetModel(Path.Combine(sourceFolder, entry.FileName));
            
            result.Checksum = entry.Checksum.ToDefaultIfEmpty();
            return result;

        }
    }
}
