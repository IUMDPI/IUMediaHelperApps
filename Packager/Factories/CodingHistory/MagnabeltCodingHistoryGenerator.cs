using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public class MagnabeltCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        private const string MagnabeltCodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1};A/D,T={2};PreAmp";

        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Format(CodingHistoryLine1Format, AnalogueFormat, MonoSoundField,
               GeneratePlayerTextField(metadata, provenance));
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Format(MagnabeltCodingHistoryLine2Format, MonoSoundField, GenerateAdTextField(provenance), GeneratePreAmpTextField(provenance));
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }

        protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }

        
    }
}