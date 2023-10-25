using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using System;
using System.Collections.Generic;

namespace Packager.Factories.CodingHistory
{

    public class DatCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        private const string Line1Format = "A={0},M={1},T={2},";
        private const string Line2Format = "A=PCM,F={0},W={1},M={2},T={3};A/D,";
        private const string Line3Format = "A=PCM,F={0},W={1},M={2},T=Lynx AES16;DIO";

        private const string ThirtyTwoK = "32k";
        private const string FortyFourOneK = "44.1k";
        private const string FortyEightK = "48k";
        private const string NinetySixK = "96k";
        private const string TwentyFourBit = "24";
        private const string SixteenBit = "16";
        private readonly IDictionary<string, string> SampleRatesToText = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ThirtyTwoK, "32000" },
            { FortyFourOneK, "44100" },
            { FortyEightK, "48000" },
            { NinetySixK, "96000" }
        };
            
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            //AssertSoundFieldSpecifiedInMetadata(metadata.SoundField);
            var ad = provenance.AnalogTransfer ? AnalogueFormat : DigitalFormat;
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;

            return string.Format(Line1Format, ad, soundField, GeneratePlayerTextField(metadata, provenance));
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;
            var bitDepth = provenance.SampleRate == NinetySixK ? TwentyFourBit : SixteenBit;
            
            return string.Format(Line2Format,ConvertSampleRate(provenance.SampleRate), bitDepth, soundField, GenerateAdTextField(provenance));
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var soundField = model.IsPreservationIntermediateVersion() ? MonoSoundField : StereoSoundField;
            var bitDepth = provenance.SampleRate == NinetySixK ? TwentyFourBit : SixteenBit;
            return string.Format(Line3Format, ConvertSampleRate(provenance.SampleRate), bitDepth, soundField);
        }

        protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Empty;
        }

        /// <summary>
        /// POD occasionally refactors without maintaining case sensitivity in its attribute values. This method should take any case of a sample rate 
        /// (48K vs 48k for instance) and look up determine the appropriate match (48000 in this example).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertSampleRate(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new EmbeddedMetadataException("Sample is blank in POD!");
            }

            return SampleRatesToText.TryGetValue(value, out var rate)
                ? rate
                : throw new EmbeddedMetadataException($"Unknown sample rate {value}");
        }
    }
}
