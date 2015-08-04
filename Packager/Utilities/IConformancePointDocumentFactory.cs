using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IConformancePointDocumentFactory
    {
        ConformancePointDocumentFile Generate(ObjectFileModel model, DigitalFileProvenance provenance, ConsolidatedPodMetadata metadata, string processingDirectory);
        
    }
}