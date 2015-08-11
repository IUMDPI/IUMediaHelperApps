using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface ISideDataFactory
    {
        SideData[] Generate(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory);
    }

    public class SideDataFactory : ISideDataFactory
    {
        public SideDataFactory(IHasher hasher, IIngestDataFactory ingestDataFactory)
        {
            Hasher = hasher;
            IngestDataFactory = ingestDataFactory;
        }

        private IHasher Hasher { get; set; }
        private IIngestDataFactory IngestDataFactory { get; set; }

        public SideData[] Generate(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
        {
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new OutputXmlException("Could not determine side groupings");
            }

            return sideGroupings.Select(grouping => new SideData
            {
                Side = grouping.Key.ToString("D2", CultureInfo.InvariantCulture),
                Files = grouping.Select(m => GetFileData(m, processingDirectory)).ToList(),
                Ingest = IngestDataFactory.Generate(podMetadata, grouping.GetPreservationOrIntermediateModel()),
                ManualCheck = "No"
            }).ToArray();
        }

        private FileData GetFileData(AbstractFileModel objectFileModel, string processingDirectory)
        {
            var filePath = Path.Combine(processingDirectory, objectFileModel.ToFileName());

            return new FileData
            {
                Checksum = Hasher.Hash(filePath),
                FileName = Path.GetFileName(filePath)
            };
        }
    }
}