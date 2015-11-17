using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Packager.Exceptions;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class EmbeddedAudioMetadataFactory : AbstractEmbeddedMetadataFactory<AudioPodMetadata>
    {
        private const string CodingHistoryLine1Format = "A={0},M={1},T={2},\r\n";
        private const string CodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1};A/D,\r\n";
        private const string CodingHistoryLine3 = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";

        private readonly List<string> _knownDigitalFormats = new List<string> { "cd-r", "dat" };
        
        protected override AbstractEmbeddedMetadata Generate(ObjectFileModel model, AbstractDigitalFile provenance, AudioPodMetadata metadata)
        {
            var description = GenerateDescription(metadata, model);

            return new EmbeddedAudioMetadata
            {
                Originator = metadata.DigitizingEntity,
                OriginatorReference = Path.GetFileNameWithoutExtension(model.ToFileName()),
                Description = description,
                ICMT = description,
                IARL = metadata.Unit,
                OriginationDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                OriginationTime = GetDateString(provenance.DateDigitized, "HH:mm:ss", ""),
                TimeReference = "0",
                ICRD = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                INAM = metadata.Title,
                CodingHistory = GenerateCodingHistory(metadata, provenance as DigitalAudioFile)
            };
        }

        private string GetFormatText(AudioPodMetadata metadata)
        {
            return _knownDigitalFormats.Contains(metadata.Format.ToLowerInvariant())
                ? "DIGITAL"
                : "ANALOGUE";
        }

        private string GenerateCodingHistory(AudioPodMetadata metadata, DigitalAudioFile provenance)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(CodingHistoryLine1Format,
                GetFormatText(metadata),
                metadata.SoundField,
                GeneratePlayerTextField(metadata, provenance));

            builder.AppendFormat(CodingHistoryLine2Format,
                metadata.SoundField,
                GenerateAdTextField(provenance));

            builder.Append(CodingHistoryLine3);

            return builder.ToString();
        }

        private static string GeneratePlayerTextField(AudioPodMetadata metadata, DigitalAudioFile provenance)
        {
            if (!provenance.PlayerDevices.Any())
            {
                throw new EmbeddedMetadataException("No player data present in metadata");
            }

            var parts = GenerateDevicePartsArray(provenance.PlayerDevices);

            parts.Add(DetermineSpeedUsed(metadata, provenance));
            parts.Add(metadata.Format);

            return string.Join(";", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
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

        private static string GenerateAdTextField(DigitalAudioFile provenance)
        {
            if (!provenance.AdDevices.Any())
            {
                throw new EmbeddedMetadataException("No AD data present in metadata");
            }

            var parts = GenerateDevicePartsArray(provenance.AdDevices);
            return string.Join(";", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        // if provenance.speedUsed is present
        // otherwise use metadata.playback speed
        // finally, replace comma delimiters with semi-colons and remove spaces
        private static string DetermineSpeedUsed(AudioPodMetadata metadata, DigitalAudioFile provenance)
        {
            var result = string.IsNullOrWhiteSpace(provenance.SpeedUsed)
                ? metadata.PlaybackSpeed
                : provenance.SpeedUsed;

            if (string.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            return result.Replace(",", ";").Replace("; ", ";");
        }
    }
}