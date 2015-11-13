using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IIngestDataFactory
    {
        IngestData Generate(AudioPodMetadata podMetadata, AbstractFileModel masterFileModel);
    }
}