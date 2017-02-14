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
using Packager.Models.SettingsModels;
using Packager.Providers;
using Packager.Utilities.Hashing;
using Packager.Utilities.Xml;
using File = Packager.Models.OutputModels.File;

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
            // 1 find images
            // 2 copy images to processing directory
            // return list of models

            // does folder exist?
            var sourceFolder = Path.Combine(ImageDirectory, $"{barcode}");
            if (DirectoryProvider.DirectoryExists(sourceFolder) == false)
            {
                // if not return empty list
                return new List<AbstractFile>();
            }

            var manifestPath = Path.Combine(sourceFolder, $"{ProjectCode}_{barcode}.xml");
            // import the manifest
            if (FileProvider.FileDoesNotExist(manifestPath))
            {
                throw new FileNotFoundException("expected manifest file to be present", manifestPath);
            }

            var manifest = XmlExporter.ImportFromFile<IU<RecordCarrier>>(manifestPath);
            var fileEntries = manifest.Carrier.Parts.Sides.SelectMany(s => s.Files);
            // get .tiff file models
            var imageFiles = fileEntries
                .Select(entry => CreateFileAndAssignHash(entry, sourceFolder))
                .Where(f => f is TiffImageFile)
                .Where(f=>f.IsValid())
                .ToList();
            
            // copy files to processing folder
            foreach (var file in imageFiles)
            {
                var sourcePath = Path.Combine(sourceFolder, file.Filename);
                var destPath = Path.Combine(BaseProcessingFolder, file.GetFolderName(), file.Filename);
                
                await FileProvider.CopyFileAsync(sourcePath, destPath, cancellationToken);
                var destChecksum = await Hasher.Hash(destPath, cancellationToken);

                if (string.Equals(file.Checksum, destChecksum, StringComparison.InvariantCultureIgnoreCase)==false)
                {
                    throw new Exception($"copy hash ({destChecksum.ToUpperInvariant()}) is not equal to original hash ({file.Checksum.ToUpperInvariant()}).");
                }

            }

            return imageFiles;
        }

        private static AbstractFile CreateFileAndAssignHash(File entry, string sourceFolder)
        {
            var result = FileModelFactory.GetModel(Path.Combine(sourceFolder, entry.FileName));
            
            result.Checksum = entry.Checksum.ToDefaultIfEmpty();
            return result;

        }
    }
}
