using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Utilities;

namespace Packager.Factories
{
    public class SideDataFactory : ISideDataFactory
    {
        public SideDataFactory(IHasher hasher, IIngestDataFactory ingestDataFactory)
        {
            Hasher = hasher;
            IngestDataFactory = ingestDataFactory;
        }

        private IHasher Hasher { get; set; }
        private IIngestDataFactory IngestDataFactory { get; set; }

        public SideData[] Generate(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess)
        {
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new OutputXmlException("Could not determine side groupings");
            }

            return sideGroupings.Select(grouping => new SideData
            {
                Side = grouping.Key.ToString("D2", CultureInfo.InvariantCulture),
                Files = grouping.Select(GetFileData).ToList(),
                Ingest = IngestDataFactory.Generate(podMetadata, grouping.GetPreservationOrIntermediateModel()),
                ManualCheck = "No"
            }).ToArray();
        }

        private FileData GetFileData(AbstractFileModel model)
        {
            return new FileData
            {
                Checksum = Hasher.Hash(model),
                FileName = model.ToFileName()
            };
        }
    }
}