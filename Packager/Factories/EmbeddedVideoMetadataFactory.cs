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

        protected override AbstractEmbeddedMetadata Generate(AbstractFile model, AbstractDigitalFile provenance, VideoPodMetadata metadata, string digitizingEntity)
        {
            return model.IsMezzanineVersion()
                ? GetForUse<EmbeddedVideoMezzanineMetadata>(provenance, metadata, digitizingEntity)
                : GetForUse<EmbeddedVideoPreservationMetadata>(provenance, metadata, digitizingEntity);
        }
        
        private AbstractEmbeddedMetadata GetForUse<T>(AbstractDigitalFile provenance,
            AbstractPodMetadata metadata, string digitizingEntity) where T : AbstractEmbeddedVideoMetadata, new()
        {
            return new T
            {
                Description = provenance.BextFile,
                Title = metadata.Title,
                MasteredDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                Comment = $"File created by {digitizingEntity}"
            };
        }
    }
}