using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    // TODO:
    // Here, the pres-int model should have their sound field set to "Mono".
    // Otherwise, the sound field should be set to "Stereo"
    public class DatCodingHistoryGenerator: AbstractCodingHistoryGenerator {
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            AssertSoundFieldSpecifiedInMetadata(metadata.SoundField);
            return string.Format(CodingHistoryLine1Format, AnalogueFormat, metadata.SoundField,
                GeneratePlayerTextField(metadata, provenance));
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            AssertSoundFieldSpecifiedInMetadata(metadata.SoundField);
            return string.Format(CodingHistoryLine2Format, metadata.SoundField, GenerateAdTextField(provenance));
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Format(CodingHistoryLine3Format, metadata.SoundField);
        }

        protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }
    }
}
