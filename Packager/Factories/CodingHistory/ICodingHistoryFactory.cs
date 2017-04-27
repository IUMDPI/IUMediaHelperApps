using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public interface ICodingHistoryFactory
    {
        string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);
    }
}