using System.Collections.Generic;
using System.IO;
using Common.Models;
using Packager.Exceptions;
using Packager.Factories.CodingHistory;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;

namespace Packager.Factories
{
    public class EmbeddedAudioMetadataFactory : AbstractEmbeddedMetadataFactory<AudioPodMetadata>
    {
        private Dictionary<IMediaFormat, ICodingHistoryGenerator> CodingHistoryGenerators { get; }

        public EmbeddedAudioMetadataFactory(IProgramSettings programSettings, 
            Dictionary<IMediaFormat, ICodingHistoryGenerator> codingHistoryGenerators ) : base(programSettings)
        {
            CodingHistoryGenerators = codingHistoryGenerators;
        }

        /// <summary>
        ///     Generate metadata to embed for a give model, provenance, and set of pod metadata
        /// </summary>
        /// <param name="model"></param>
        /// <param name="provenance"></param>
        /// <param name="metadata"></param>
        /// <param name="digitizingEntity"></param>
        /// <returns></returns>
        protected override AbstractEmbeddedMetadata Generate(AbstractFile model, AbstractDigitalFile provenance,
            AudioPodMetadata metadata, string digitizingEntity)
        {
            return new EmbeddedAudioMetadata
            {
                Originator = digitizingEntity,
                OriginatorReference = Path.GetFileNameWithoutExtension(model.Filename),
                Description = provenance.BextFile,
                ICMT = provenance.BextFile,
                IARL = metadata.Iarl,
                OriginationDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                OriginationTime = GetDateString(provenance.DateDigitized, "HH:mm:ss", ""),
                TimeReference = "0",
                ICRD = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                INAM = metadata.Title,
                CodingHistory = GenerateCodingHistory(metadata, provenance as DigitalAudioFile, model)
            };
        }

        private string GenerateCodingHistory(AudioPodMetadata metadata, DigitalAudioFile provenance, AbstractFile model)
        {
            if (!CodingHistoryGenerators.ContainsKey(metadata.Format))
            {
                throw new EmbeddedMetadataException(
                    $"No coding history generator defined for {metadata.Format}");
            }

            return CodingHistoryGenerators[metadata.Format].Generate(metadata, provenance, model);
        }
    }
}