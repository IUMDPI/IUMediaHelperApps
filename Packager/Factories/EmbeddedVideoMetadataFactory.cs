using Packager.Extensions;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;

namespace Packager.Factories
{
    public class EmbeddedVideoMetadataFactory : AbstractEmbeddedMetadataFactory<VideoPodMetadata>
    {
        public EmbeddedVideoMetadataFactory(IProgramSettings programSettings) : base(programSettings)
        {
        }

        protected override AbstractEmbeddedMetadata Generate(AbstractFile model, AbstractDigitalFile provenance,
            VideoPodMetadata metadata)
        {
            return model.IsMezzanineVersion()
                ? GetForUse<EmbeddedVideoMezzanineMetadata>(model, provenance, metadata)
                : GetForUse<EmbeddedVideoPreservationMetadata>(model, provenance, metadata);
        }


        private AbstractEmbeddedMetadata GetForUse<T>(AbstractFile model, AbstractDigitalFile provenance,
            AbstractPodMetadata metadata) where T : AbstractEmbeddedVideoMetadata, new()
        {
            return new T
            {
                Description = GenerateDescription(metadata, model),
                Title = metadata.Title,
                MasteredDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                Comment = $"File created by {metadata.DigitizingEntity}"
            };
        }
    }
}