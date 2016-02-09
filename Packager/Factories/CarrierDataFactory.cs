using System.Collections.Generic;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class VideoCarrierDataFactory : ICarrierDataFactory
    {
        public VideoCarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; }

        public AbstractCarrierData Generate(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            return new VideoCarrier
            {
                Barcode = metadata.Barcode,
                CarrierType = "Video", // todo: is this correct
                Identifier = metadata.CallNumber.ToDefaultIfEmpty("Unknown"),
                Baking = new BakingData {Date = metadata.BakingDate},
                Cleaning = new CleaningData {Date = metadata.CleaningDate, Comment = metadata.CleaningComment},
                Parts = GeneratePartsData((VideoPodMetadata) metadata, filesToProcess),
                PhysicalCondition = new PhysicalConditionData
                {
                    Damage = metadata.Damage,
                    PreservationProblem = metadata.PreservationProblems
                },
                ImageFormat = ((VideoPodMetadata) metadata).ImageFormat,
                RecordingStandard = ((VideoPodMetadata) metadata).RecordingStandard,
                Preview = new PreviewData(),
                Definition = "SD", // todo: figure out where to get this from
                Comments = metadata.Comments
            };
        }

        private PartsData GeneratePartsData(VideoPodMetadata metadata, IEnumerable<AbstractFile> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = metadata.DigitizingEntity,
                Sides = SideDataFactory.Generate(metadata, filesToProcess)
            };
        }
    }

    public class AudioCarrierDataFactory : ICarrierDataFactory
    {
        public AudioCarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; }

        public AbstractCarrierData Generate(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            var result = new AudioCarrier
            {
                Barcode = metadata.Barcode,
                Brand = ((AudioPodMetadata) metadata).Brand,
                CarrierType = metadata.Format,
                DirectionsRecorded = ((AudioPodMetadata) metadata).DirectionsRecorded,
                Identifier = metadata.CallNumber,
                Thickness = ((AudioPodMetadata) metadata).TapeThickness,
                Baking = new BakingData {Date = metadata.BakingDate},
                Cleaning = new CleaningData {Date = metadata.CleaningDate, Comment = metadata.CleaningComment},
                Repaired = metadata.Repaired,
                Parts = GeneratePartsData((AudioPodMetadata) metadata, filesToProcess),
                Configuration = new ConfigurationData
                {
                    XsiType = $"Configuration{metadata.Format}".RemoveSpaces(),
                    Track = ((AudioPodMetadata) metadata).TrackConfiguration,
                    SoundField = ((AudioPodMetadata) metadata).SoundField
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