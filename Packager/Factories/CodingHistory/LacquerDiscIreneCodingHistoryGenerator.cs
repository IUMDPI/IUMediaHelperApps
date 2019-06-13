using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public class LacquerDiscIreneCodingHistoryGenerator : AbstractCodingHistoryGenerator
    {
        private const string Line1 = "A=ANALOG,M=Mono,T=IRENE;Lacquer Disc,";
        private const string Line2 = "A=TIFF,M=Mono,T=IRENE imaging sensor,";
        private const string Line3 = "A=PCM,F=130000,W=32fp,M=Mono,T=Weaver software,";
        private const string Line4 = "A=PCM,F=96000,W=24,M=Mono,T=Izotope RX6";
        
        protected override string GenerateLine1(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return Line1;
        }

        protected override string GenerateLine2(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return Line2;
        }

        protected override string GenerateLine3(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return Line3;
        }

         protected override string GenerateLine4(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            return Line4;
        }
    }
}
