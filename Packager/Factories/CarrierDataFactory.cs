using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;

namespace Packager.Factories
{
    public class CarrierDataFactory : ICarrierDataFactory
    {
        public CarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; set; }
        
        public CarrierData Generate(ConsolidatedPodMetadata podMetadata, List<ObjectFileModel> filesToProcess)
        {
            var result = new CarrierData
            {
                Barcode = podMetadata.Barcode,
                Brand = podMetadata.Brand,
                CarrierType = podMetadata.Format,
                XsiType = $"{podMetadata.Format}Carrier".RemoveSpaces(),
                DirectionsRecorded = podMetadata.DirectionsRecorded,
                Identifier = podMetadata.CallNumber,
                Thickness = podMetadata.TapeThickness,
                Baking = new BakingData {Date = podMetadata.BakingDate},
                Cleaning = new CleaningData {Date = podMetadata.CleaningDate, Comment = podMetadata.CleaningComment},
                Repaired = podMetadata.Repaired,
                Parts = GeneratePartsData(podMetadata, filesToProcess),
                Configuration = new ConfigurationData
                {
                    XsiType = $"Configuration{podMetadata.Format}".RemoveSpaces(),
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

        private PartsData GeneratePartsData(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = podMetadata.DigitizingEntity,
                Sides = SideDataFactory.Generate(podMetadata, filesToProcess)
            };
        }
    }
}