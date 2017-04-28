using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

namespace Packager.Factories
{
    public class VideoCarrierDataFactory : ICarrierDataFactory<VideoPodMetadata>
    {
        public VideoCarrierDataFactory(ISideDataFactory sideDataFactory, IMediaInfoProvider mediaInfoProvider)
        {
            SideDataFactory = sideDataFactory;
            MediaInfoProvider = mediaInfoProvider;
        }

        private ISideDataFactory SideDataFactory { get; }
        private IMediaInfoProvider MediaInfoProvider { get; }

        public async Task<AbstractCarrierData> Generate(VideoPodMetadata metadata, string digitizingEntity, List<AbstractFile> filesToProcess, CancellationToken cancellationToken)
        {
            return new VideoCarrier
            {
                Barcode = metadata.Barcode,
                CarrierType = "Video", 
                Identifier = metadata.CallNumber.ToDefaultIfEmpty("Unknown"),
                Baking = new BakingData {Date = metadata.BakingDate?.ToString("yyyy-MM-dd") },
                Cleaning = new CleaningData {Date = metadata.CleaningDate?.ToString("yyyy-MM-dd"), Comment = metadata.CleaningComment},
                Parts = GeneratePartsData(metadata, digitizingEntity, filesToProcess),
                PhysicalCondition = new PhysicalConditionData
                {
                    Damage = metadata.Damage,
                    PreservationProblem = metadata.PreservationProblems
                },
                Configuration = await GetConfiguration(metadata.Format, filesToProcess.GetPreservationOrIntermediateModel(), cancellationToken),
                ImageFormat = metadata.ImageFormat,
                RecordingStandard = metadata.RecordingStandard,
                Preview = new PreviewData(),
                Definition = "SD",
                Comments = metadata.Comments
               
            };
        }

        private async Task<VideoConfigurationData> GetConfiguration(IMediaFormat format, AbstractFile model, CancellationToken cancellationToken)
        {
            if (format != MediaFormats.Betamax)
            {
                return new VideoConfigurationData
                {
                    DigitalFileConfigurationVariant = string.Empty
                };
            }

            var mediaInfo =await MediaInfoProvider.GetMediaInfo(model, cancellationToken);

            var variant = mediaInfo.AudioStreams > 2
                ? "4StreamAudio"
                : string.Empty;

            return new VideoConfigurationData
            {
                DigitalFileConfigurationVariant = variant
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