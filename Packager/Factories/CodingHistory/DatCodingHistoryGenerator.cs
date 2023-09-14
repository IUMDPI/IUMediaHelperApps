using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    // TODO:
    // Here, the pres-int model should have their sound field set to "Mono".
    // Otherwise, the sound field should be set to "Stereo"
    public class DatCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            //AssertSoundFieldSpecifiedInMetadata(metadata.SoundField);
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;
            return string.Format(CodingHistoryLine1Format, DigitalFormat, soundField,
                GeneratePlayerTextField(metadata, provenance));
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;
            return string.Format(CodingHistoryLine2Format, soundField, GenerateAdTextField(provenance));
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;
            return string.Format(CodingHistoryLine3Format, soundField);
        }

        protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }
    }
}
