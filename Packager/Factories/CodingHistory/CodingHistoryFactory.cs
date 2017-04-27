using System.Collections.Generic;
using Packager.Exceptions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories.CodingHistory
{
    public class CodingHistoryFactory : ICodingHistoryFactory
    {
        private readonly Dictionary<string, ICodingHistoryGenerator> _generators =
            new Dictionary<string, ICodingHistoryGenerator>
            {
                { "open reel audio tape", new OpenReelCodingHistoryGenerator()},
                { "lacquer disc", new LacquerOrCylinderCodingHistoryGenerator()},
                {"cylinder", new LacquerOrCylinderCodingHistoryGenerator() } 
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
}
