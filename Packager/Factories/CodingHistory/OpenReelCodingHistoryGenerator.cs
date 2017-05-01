using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public class OpenReelCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
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

        private static void AssertSoundFieldSpecifiedInMetadata(string soundField)
        {
            if (soundField.IsSet())
            {
                return;
            }

            throw new EmbeddedMetadataException("No sound field specified in metadata");
        }
    }
}