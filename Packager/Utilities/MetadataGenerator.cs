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
                CarrierType = podMetadata.Format,
                XsiType = string.Format("{0}Carrier", podMetadata.Format),
                DirectionsRecorded = podMetadata.DirectionsRecorded,
                Identifier = podMetadata.Identifier,
                Thickness = podMetadata.TapeThickness,
                Baking = new BakingData { Date = podMetadata.BakingDate },
                Cleaning = new CleaningData { Date = podMetadata.CleaningDate, Comment = podMetadata.CleaningComment},
                Repaired = podMetadata.Repaired,
                Parts = GeneratePartsData(podMetadata, filesToProcess, processingDirectory),
                Configuration = new ConfigurationData
                {
                    XsiType = string.Format("Configuration{0}", podMetadata.Format),
                    Track = podMetadata.TrackConfiguration,
                    SoundField = podMetadata.SoundField,
                    Speed = podMetadata.PlaybackSpeed
                },
                PhysicalCondition = new PhysicalConditionData
                {
                    Damage = podMetadata.Damage,
                    PreservationProblem = podMetadata.PreservationProblems
                }
            };

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
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new OutputXmlException("Could not determine side groupings");
            }

            return sideGroupings.Select(grouping => new SideData
            {
                Side = grouping.Key.ToString("D2", CultureInfo.InvariantCulture),
                Files = grouping.Select(m => GetFileData(m, processingDirectory)).ToList(),
                Ingest = GenerateIngestMetadata(podMetadata, grouping.GetPreservationOrIntermediateModel()),
                ManualCheck = "No"
            }).ToArray();
        }

        private static IngestData GenerateIngestMetadata(ConsolidatedPodMetadata podMetadata, AbstractFileModel masterFileModel)
        {
            var digitalFileProvenance = podMetadata.FileProvenances.GetFileProvenance(masterFileModel);
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.ToFileName());
            }

            return new IngestData
            {
                XsiType = string.Format("{0}Ingest", podMetadata.Format),
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
                Date = digitalFileProvenance.DateDigitized
            };
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