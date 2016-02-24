using Packager.Models.OutputModels.Ingest;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IIngestDataFactory
    {
        AbstractIngest Generate(DigitalAudioFile provenance);
        AbstractIngest Generate(DigitalVideoFile provenance);
    }
}