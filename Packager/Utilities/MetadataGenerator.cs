using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public class MetadataGenerator : IMetadataGenerator
    {
        public MetadataGenerator(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; set; }
        
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
                Baking = new BakingData {Date = podMetadata.BakingDate},
                Cleaning = new CleaningData {Date = podMetadata.CleaningDate, Comment = podMetadata.CleaningComment},
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
                Sides = SideDataFactory.Generate(podMetadata, filesToProcess, processingDirectory)
            };
        }
    }
}