using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICodingHistoryFactory
    {
        string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);
    }

    public interface ICodingHistoryGenerator
    {
        string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);
    }

    public class CodingHistoryFactory : ICodingHistoryFactory
    {
        private readonly Dictionary<string, ICodingHistoryGenerator> _generators =
            new Dictionary<string, ICodingHistoryGenerator>
            {
                { "open reel audio tape", new OpenReelCodingHistoryGenerator()} 
            };


        public string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            if (!_generators.ContainsKey(metadata.Format.ToLowerInvariant()))
            {
                throw new EmbeddedMetadataException(
                    $"No coding history generator defined for {metadata.Format}");
            }

            return _generators[metadata.Format.ToLowerInvariant()].Generate(metadata, provenance, model);
        }


    }

    public abstract class AbstractCodingHistoryGenerator : ICodingHistoryGenerator
    {
        protected const string AnalogueFormat = "ANALOGUE";
        protected const string DigitalFormat = "DIGITAL";
        protected const string MonoSoundField = "mono";
        protected const string StereoSoundField = "stereo";

        protected const string CodingHistoryLine1Format = "A={0},M={1},T={2},\r\n";
        protected const string CodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1};A/D,\r\n";
        protected const string CodingHistoryLine3Format = "A=PCM,F=96000,W=24,M={0},T=Lynx AES16;DIO";

        protected abstract string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        protected abstract string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        protected abstract string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        public string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var builder = new StringBuilder();
            builder.Append(GenerateLine1(metadata, provenance, model));
            builder.Append(GenerateLine2(metadata, provenance, model));
            builder.Append(GenerateLine3(metadata, provenance, model));
            return builder.ToString();
        }

        protected static string GeneratePlayerTextField(AbstractPodMetadata metadata, DigitalAudioFile provenance)
        {
            if (!provenance.PlayerDevices.Any())
            {
                throw new EmbeddedMetadataException("No player data present in metadata");
            }

            var parts = GenerateDevicePartsArray(provenance.PlayerDevices);

            parts.Add(DetermineSpeedUsed(provenance));
            parts.Add(metadata.Format);

            return string.Join(";", parts.Where(p => p.IsSet()));
        }

        private static List<string> GenerateDevicePartsArray(IEnumerable<Device> devices)
        {
            var parts = new List<string>();

            foreach (var device in devices)
            {
                parts.Add($"{device.Manufacturer} {device.Model}");
                parts.Add($"SN{device.SerialNumber}");
            }

            return parts;
        }

        protected static string GenerateAdTextField(AbstractDigitalFile provenance)
        {
            if (!provenance.AdDevices.Any())
            {
                throw new EmbeddedMetadataException("No AD data present in metadata");
            }

            var parts = GenerateDevicePartsArray(provenance.AdDevices);
            return string.Join(";", parts.Where(p => p.IsSet()));
        }

        // if provenance.speedUsed is present
        // otherwise use metadata.playback speed
        // finally, replace comma delimiters with semi-colons and remove spaces
        private static string DetermineSpeedUsed(DigitalAudioFile provenance)
        {
            var result = provenance.SpeedUsed;

            return result.IsNotSet()
                ? result
                : result.Replace(",", ";").Replace("; ", ";");
        }
    }

    public class LacquerOrCylinderCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return string.Format(CodingHistoryLine1Format, AnalogueFormat, MonoSoundField,
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
    }

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
            return string.Format(CodingHistoryLine3Format, MonoSoundField);
        }

        private void AssertSoundFieldSpecifiedInMetadata(string soundField)
        {
            if (soundField.IsSet())
            {
                return;
            }

            throw new EmbeddedMetadataException("No sound field specified in metadata");
        }
    }

    

}
