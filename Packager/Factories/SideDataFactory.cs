using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;

namespace Packager.Factories
{
    public class SideDataFactory : ISideDataFactory
    {
        public SideDataFactory(IIngestDataFactory ingestDataFactory)
        {
            IngestDataFactory = ingestDataFactory;
        }

        private IIngestDataFactory IngestDataFactory { get; }

        public SideData[] Generate(ConsolidatedAudioPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess)
        {
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new OutputXmlException("Could not determine side groupings");
            }

            return sideGroupings.Select(grouping => new SideData
            {
                Side = grouping.Key.ToString(CultureInfo.InvariantCulture),
                Files = grouping.Select(GetFileData).ToList(),
                Ingest = IngestDataFactory.Generate(podMetadata, grouping.GetPreservationOrIntermediateModel()),
                ManualCheck = "No"
            }).ToArray();
        }

        private static File GetFileData(ObjectFileModel model)
        {
            return new File
            {
                Checksum = model.Checksum,
                FileName = model.ToFileName()
            };
        }
    }
}