using System;
using System.Collections.Generic;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class CarrierDataFactory : ICarrierDataFactory
    {
        public CarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; set; }
        
        private AudioCarrier Generate(AudioPodMetadata metadata, List<AbstractFile> filesToProcess)
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
                    SoundField = metadata.SoundField,
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

        private VideoCarrier Generate(VideoPodMetadata metadata, List<AbstractFile> filesToProcess)
        {
            var result = new VideoCarrier
            {
                Barcode = metadata.Barcode,
                CarrierType = "Video", // todo: is this correct
                Identifier = metadata.CallNumber.ToDefaultIfEmpty("Unknown"),
                Baking = new BakingData { Date = metadata.BakingDate },
                Cleaning = new CleaningData { Date = metadata.CleaningDate, Comment = metadata.CleaningComment },
                Parts = GeneratePartsData(metadata, filesToProcess),
                PhysicalCondition = new PhysicalConditionData
                {
                    Damage = metadata.Damage,
                    PreservationProblem = metadata.PreservationProblems
                },
                ImageFormat = metadata.ImageFormat,
                RecordingStandard = metadata.RecordingStandard,
                Preview = new PreviewData(),
                Definition = "SD", // todo: figure out where to get this from
                Comments = metadata.Comments
            };

            return result;
        }

        public T Generate<T>(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess) where T : AbstractCarrierData
        {
            var audioPodMetadata = metadata as AudioPodMetadata;
            if (audioPodMetadata != null)
            {
                return Generate(audioPodMetadata, filesToProcess) as T;
            }

            var videoPodMetadata = metadata as VideoPodMetadata;
            if (videoPodMetadata != null)
            {
                return Generate(videoPodMetadata, filesToProcess) as T;
            }

            throw new NotImplementedException($"No carrier data generator defined for {metadata.GetType().Name}");
        }

        private PartsData GeneratePartsData(AudioPodMetadata metadata, IEnumerable<AbstractFile> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = metadata.DigitizingEntity,
                Sides = SideDataFactory.Generate(metadata, filesToProcess)
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
}