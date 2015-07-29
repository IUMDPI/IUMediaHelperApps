using System;
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
    public class MetadataGenerator : IMetadataGenerator
    {
        public MetadataGenerator(IHasher hasher)
        {
            Hasher = hasher;
        }

        private IHasher Hasher { get; set; }

        public CarrierData GenerateMetadata(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
        {
            var result = new CarrierData
            {
                Barcode = podMetadata.Barcode,
                Brand = podMetadata.Brand,
                CarrierType = podMetadata.CarrierType,
                DirectionsRecorded = podMetadata.DirectionsRecorded,
                Identifier = podMetadata.Identifier,
                Thickness = podMetadata.TapeThicknesses,
                Baking = new BakingData {Date = podMetadata.BakingDate},
                Cleaning = new CleaningData {Date = podMetadata.CleaningDate},
                Repaired = podMetadata.Repaired ? "Yes" : "No",
                Parts = GeneratePartsData(podMetadata, filesToProcess, processingDirectory),
                Configuration = ConfigurationData.FromPodMetadata(podMetadata)
            }; //.FromPodMetadata(podMetadata);


            return result;
        }

        private PartsData GeneratePartsData(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
        {
            return new PartsData
            {
                DigitizingEntity = podMetadata.DigitizingEntity,
                Sides = GenerateSideData(podMetadata, filesToProcess, processingDirectory)
            };
        }

        private SideData[] GenerateSideData(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
        {
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator.ToInteger()).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new OutputXmlException("Could not determine side groupings");
            }

            var result = new List<SideData>();
            foreach (var grouping in sideGroupings)
            {
                if (!grouping.Key.HasValue)
                {
                    throw new OutputXmlException("One or more groupings has an invalid sequence value");
                }

                var sideData = new SideData
                {
                    Side = grouping.Key.Value.ToString(CultureInfo.InvariantCulture),
                    Files = grouping.Select(m => GetFileData(m, processingDirectory)).ToList(),
                    Ingest = GenerateIngestMetadata(podMetadata, grouping.GetPreservationOrIntermediateModel()),
                    ManualCheck = "No" //todo: where to get value from?
                };

                result.Add(sideData);
            }

            return result.ToArray();
        }

        private static IngestData GenerateIngestMetadata(ConsolidatedPodMetadata podMetadata, ObjectFileModel masterFileModel)
        {
            var digitalFileProvenance = podMetadata.DigitalProvenance.DigitalFileProvenances
                .SingleOrDefault(f => f.Filename.Equals(masterFileModel.ToFileName(), StringComparison.InvariantCultureIgnoreCase));
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.ToFileName());
            }

            return new IngestData
            {
                AdManufacturer = digitalFileProvenance.AdManufacturer,
                AdModel = digitalFileProvenance.AdModel,
                AdSerialNumber = digitalFileProvenance.AdSerialNumber,
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                PlayerManufacturer = digitalFileProvenance.PlayerManufacturer,
                PlayerModel = digitalFileProvenance.PlayerModel,
                PlayerSerialNumber = digitalFileProvenance.PlayerSerialNumber,
                ExtractionWorkstation = digitalFileProvenance.ExtractionWorkstation,
                SpeedUsed = digitalFileProvenance.SpeedUsed,
                Date = digitalFileProvenance.CreatedAt,
            };
        }

        private FileData GetFileData(AbstractFileModel objectFileModel, string processingDirectory)
        {
            var filePath = Path.Combine(processingDirectory, objectFileModel.ToFileName());
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new FileData
                {
                    Checksum = Hasher.Hash(stream),
                    FileName = Path.GetFileName(filePath)
                };
            }
        }
    }
}