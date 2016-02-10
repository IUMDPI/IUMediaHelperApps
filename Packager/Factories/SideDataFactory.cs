using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class SideDataFactory : ISideDataFactory
    {
        public SideDataFactory(IIngestDataFactory ingestDataFactory)
        {
            IngestDataFactory = ingestDataFactory;
        }

        private IIngestDataFactory IngestDataFactory { get; }

        public SideData[] Generate(AudioPodMetadata podMetadata, IEnumerable<AbstractFile> filesToProcess)
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

        public SideData[] Generate(VideoPodMetadata podMetadata, IEnumerable<AbstractFile> filesToProcess)
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
                ManualCheck = "No", // todo: ok?
                QCStatus = "OK" // todo: ok?
            }).ToArray();
        }

        private static File GetFileData(AbstractFile model)
        {
            return new File
            {
                Checksum = model.Checksum,
                FileName = model.Filename
            };
        }
    }
}