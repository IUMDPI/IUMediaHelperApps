using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public class SeventyEightCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            AssertSoundFieldSpecifiedInMetadata(metadata.SoundField);
            return string.Format(CodingHistoryLine1Format, AnalogueFormat, metadata.SoundField,
                GeneratePlayerTextField(metadata, provenance));
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationVersion() ? StereoSoundField : MonoSoundField;
            return string.Format(CodingHistoryLine2Format, soundField, GenerateAdTextField(provenance));
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationVersion() ? StereoSoundField : MonoSoundField;
            return string.Format(CodingHistoryLine3Format, soundField);
        }

        protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }
    }
}
