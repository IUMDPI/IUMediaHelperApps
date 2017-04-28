using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

#pragma warning disable 1998
        public async Task<AbstractCarrierData> Generate(AudioPodMetadata metadata, string digitizingEntity, List<AbstractFile> filesToProcess, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            var result = new AudioCarrier
            {
                Barcode = metadata.Barcode,
                Brand = metadata.Brand,
                CarrierType = metadata.Format.ProperName,
                DirectionsRecorded = metadata.DirectionsRecorded,
                Identifier = metadata.CallNumber.ToDefaultIfEmpty("Unknown"),
                Thickness = metadata.TapeThickness,
                Baking = new BakingData {Date = metadata.BakingDate?.ToString("yyyy-MM-dd") },
                Cleaning = new CleaningData {Date = metadata.CleaningDate?.ToString("yyyy-MM-dd"), Comment = metadata.CleaningComment},
                Repaired = metadata.Repaired,
                Parts = GeneratePartsData(metadata, digitizingEntity, filesToProcess),
                Configuration = new AudioConfigurationData
                {
                    Track = metadata.TrackConfiguration,
                    SoundField = metadata.SoundField,
                    Speed = metadata.PlaybackSpeed,
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

        private PartsData GeneratePartsData(AudioPodMetadata metadata, string digitizingEntity, IEnumerable<AbstractFile> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = digitizingEntity,
                Sides = SideDataFactory.Generate(metadata, filesToProcess)
            };
        }
    }
}