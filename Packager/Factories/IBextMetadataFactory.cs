using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IBextMetadataFactory
    {
        BextMetadata Generate(ObjectFileModel model, DigitalFileProvenance provenance, ConsolidatedPodMetadata metadata);
        
    }
}