using System.Text;
using Packager.Models.PodMetadataModels;

namespace Packager.Models.BextModels
{
    public class CodingHistory
    {
        private const string Line1Format = "A={0},M={1},T={2} {3};{4};{5};{6},\r\n";
        private const string Line2Format = "A=PCM,F=96000,W=24,M={0},T={1} {2};{3};A/D,\r\n";
        private const string Line3 = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";

        public CodingHistory(ConsolidatedPodMetadata podMetadata, DigitalFileProvenance provenance)
        {
            PodMetadata = podMetadata;
            Provenance = provenance;
        }

        private ConsolidatedPodMetadata PodMetadata { get; set; }
        private DigitalFileProvenance Provenance { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat(Line1Format,
                PodMetadata.IsDigitalFormat() ? "DIGITAL" : "ANALOG",
                PodMetadata.SoundField,
                Provenance.PlayerManufacturer,
                Provenance.PlayerModel,
                Provenance.PlayerSerialNumber,
                PodMetadata.PlaybackSpeed,
                PodMetadata.Format);

            builder.AppendFormat(Line2Format,
                PodMetadata.SoundField,
                Provenance.AdManufacturer,
                Provenance.AdModel,
                Provenance.AdSerialNumber);

            builder.Append(Line3);

            return builder.ToString();
        }
    }
}