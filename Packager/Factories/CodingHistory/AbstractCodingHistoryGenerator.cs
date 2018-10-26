using System.Collections.Generic;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using RestSharp.Extensions;

namespace Packager.Factories.CodingHistory
{
    public abstract class AbstractCodingHistoryGenerator : ICodingHistoryGenerator
    {
        protected const string AnalogueFormat = "ANALOGUE";
        protected const string DigitalFormat = "DIGITAL";
        protected const string MonoSoundField = "Mono";
        protected const string StereoSoundField = "Stereo";

        protected const string CodingHistoryLine1Format = "A={0},M={1},T={2},";
        protected const string CodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1};A/D,";
        protected const string CodingHistoryLine3Format = "A=PCM,F=96000,W=24,M={0},T=Lynx AES16;DIO";

        protected abstract string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        protected abstract string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        protected abstract string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model);

        public string Generate(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            var parts = new[]{
                GenerateLine1(metadata, provenance, model),
                GenerateLine2(metadata, provenance, model),
                GenerateLine3(metadata, provenance, model)};

            return string.Join("\r\n", parts.Where(p=>p.HasValue()));
        }

        protected static string GeneratePlayerTextField(AbstractPodMetadata metadata, DigitalAudioFile provenance)
        {
            if (!provenance.PlayerDevices.Any())
            {
                throw new EmbeddedMetadataException("No player data present in metadata");
            }

            var parts = GenerateDevicePartsArray(provenance.PlayerDevices);

            parts.Add(DetermineSpeedUsed(provenance));
            parts.Add(metadata.Format.ProperName);

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

            return parts
                .Where(p=>p.HasValue())
                .Select(p=>p.Trim())
                .ToList();
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
}