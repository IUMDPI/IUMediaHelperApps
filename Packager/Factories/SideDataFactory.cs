using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Ingest;
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
                Files = grouping.OrderBy(model=>model.Precedence).Select(GetFileData).ToList(),
                Ingest = GenerateAudioIngestElements(podMetadata, grouping),
                ManualCheck = "No"
            }).ToArray();
        }

        private List<AbstractIngest> GenerateAudioIngestElements(AbstractPodMetadata podMetadata,
            IEnumerable<AbstractFile> models)
        {
            return GetMatchingProvenances<DigitalAudioFile>(podMetadata, models)
               .Select(IngestDataFactory.Generate)
               .ToList();
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
                Files = grouping.OrderBy(model => model.Precedence).Select(GetFileData).ToList(),
                Ingest = GenerateVideoIngestElements(podMetadata, grouping),
                ManualCheck = "No", // todo: ok?
                QCStatus = "OK" // todo: ok?
            }).ToArray();
        }

        private List<AbstractIngest> GenerateVideoIngestElements(AbstractPodMetadata podMetadata,
           IEnumerable<AbstractFile> models)
        {
            return GetMatchingProvenances<DigitalVideoFile>(podMetadata, models)
               .Select(IngestDataFactory.Generate)
               .ToList();
        }

        private static List<T> GetMatchingProvenances<T>(AbstractPodMetadata podMetadata,
            IEnumerable<AbstractFile> filesToProcess) where T : AbstractDigitalFile
        {
            // intersect digital provenances with applicable models
            return filesToProcess
                .OrderBy(model => model.Precedence)
                .Select(model => podMetadata.FileProvenances.GetFileProvenance(model) as T)
                .Where(provenance => provenance != null).ToList();
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