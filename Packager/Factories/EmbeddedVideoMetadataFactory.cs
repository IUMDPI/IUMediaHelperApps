using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class EmbeddedVideoMetadataFactory : AbstractEmbeddedMetadataFactory<VideoPodMetadata>
    {
        protected override AbstractEmbeddedMetadata Generate(ObjectFileModel model, AbstractDigitalFile provenance, VideoPodMetadata metadata)
        {
            return model.IsMezzanineVersion()
                ? GetForUse<EmbeddedVideoMezzanineMetadata>(model, provenance, metadata)
                : GetForUse<EmbeddedVideoPreservationMetadata>(model, provenance, metadata);
        }


        private AbstractEmbeddedMetadata GetForUse<T>(ObjectFileModel model, AbstractDigitalFile provenance, VideoPodMetadata metadata) where T : AbstractEmbeddedVideoMetadata, new()
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