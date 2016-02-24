using System.Collections.Generic;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class VideoCarrierDataFactory : ICarrierDataFactory<VideoPodMetadata>
    {
        public VideoCarrierDataFactory(ISideDataFactory sideDataFactory)
        {
            SideDataFactory = sideDataFactory;
        }

        private ISideDataFactory SideDataFactory { get; }

        public AbstractCarrierData Generate(VideoPodMetadata metadata, string digitizingEntity, List<AbstractFile> filesToProcess)
        {
            return new VideoCarrier
            {
                Barcode = metadata.Barcode,
                CarrierType = "Video", // todo: is this correct
                Identifier = metadata.CallNumber.ToDefaultIfEmpty("Unknown"),
                Baking = new BakingData {Date = metadata.BakingDate?.ToString("yyyy-MM-dd") },
                Cleaning = new CleaningData {Date = metadata.CleaningDate?.ToString("yyyy-MM-dd"), Comment = metadata.CleaningComment},
                Parts = GeneratePartsData(metadata, digitizingEntity, filesToProcess),
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
        }

        private PartsData GeneratePartsData(VideoPodMetadata metadata, string digitizingEntity, IEnumerable<AbstractFile> filesToProcess)
        {
            return new PartsData
            {
                DigitizingEntity = digitizingEntity,
                Sides = SideDataFactory.Generate(metadata, filesToProcess)
            };
        }
    }
}