using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Ingest;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IIngestDataFactory
    {
        AudioIngest Generate(AudioPodMetadata podMetadata, AbstractFileModel masterFileModel);
        VideoIngest Generate(VideoPodMetadata podMetadata, AbstractFileModel masterFileModel);
    }
}