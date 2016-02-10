using System.Collections.Generic;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class AudioCarrierDataFactory : ICarrierDataFactory<AudioPodMetadata>
    {
        public AudioCarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; }

        public AbstractCarrierData Generate(AudioPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            var result = new AudioCarrier
            {
                Barcode = metadata.Barcode,
                Brand = metadata.Brand,
                CarrierType = metadata.Format,
                DirectionsRecorded = metadata.DirectionsRecorded,
                Identifier = metadata.CallNumber,
                Thickness = metadata.TapeThickness,
                Baking = new BakingData {Date = metadata.BakingDate},
                Cleaning = new CleaningData {Date = metadata.CleaningDate, Comment = metadata.CleaningComment},
                Repaired = metadata.Repaired,
                Parts = GeneratePartsData(metadata, filesToProcess),
                Configuration = new ConfigurationData
                {
                    XsiType = $"Configuration{metadata.Format}".RemoveSpaces(),
                    Track = metadata.TrackConfiguration,
                    SoundField = metadata.SoundField
                    //Speed = metadata.PlaybackSpeed,
                },
                PhysicalCondition = new PhysicalConditionData
                {
                    Damage = metadata.Damage,
                    PreservationProblem = metadata.PreservationProblems
                },
                Comments = metadata.Comments
            };

            return result;
        }

        private PartsData GeneratePartsData(AudioPodMetadata metadata, IEnumerable<AbstractFile> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = metadata.DigitizingEntity,
                Sides = SideDataFactory.Generate(metadata, filesToProcess)
            };
        }
    }
}