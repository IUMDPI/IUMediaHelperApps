using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class EmbeddedVideoMetadataFactory : AbstractEmbeddedMetadataFactory<VideoPodMetadata>
    {
        protected override AbstractEmbeddedMetadata Generate(ObjectFileModel model, AbstractDigitalFile provenance, VideoPodMetadata metadata)
        {
            return new EmbeddedVideoMetadata
            {
                Description = GenerateDescription(metadata, model),
                Title = metadata.Title,
                MasteredDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                Comment = $"File created by {metadata.DigitizingEntity}"
            };
        }
    }
}