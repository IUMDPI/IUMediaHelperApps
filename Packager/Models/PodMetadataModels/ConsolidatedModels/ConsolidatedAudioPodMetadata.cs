using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class ConsolidatedAudioPodMetadata : AbstractConsolidatedPodMetadata
    {
        public string Brand { get; set; }
        public string DirectionsRecorded { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }
        
        public override void ImportFromFullMetadata(PodMetadata metadata)
        {
            base.ImportFromFullMetadata(metadata);

            Brand = metadata.Data.Object.TechnicalMetadata.TapeStockBrand;
            DirectionsRecorded = metadata.Data.Object.TechnicalMetadata.DirectionsRecorded;
            PlaybackSpeed = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PlaybackSpeed);
            TrackConfiguration = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TrackConfiguration);
            SoundField = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.SoundField);
            TapeThickness = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TapeThickness);
            FileProvenances = new List<AbstractConsolidatedDigitalFile>();
            
        }

        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(IEnumerable<DigitalFileProvenance> originals)
        {
            return originals.Select(provenance => new ConsolidatedDigitalAudioFile(provenance))
                .Cast<AbstractConsolidatedDigitalFile>().ToList();
        }

        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(XDocument document)
        {
            return document.ImportFromChildNodes<ConsolidatedDigitalAudioFile>("/pod/data/object/digital_provenance/digital_files")
                .Select(e=>e as AbstractConsolidatedDigitalFile).ToList();
        }
    }
}