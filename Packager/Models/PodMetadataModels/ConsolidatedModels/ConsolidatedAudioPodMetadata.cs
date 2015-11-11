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

        public override void ImportFromXml(XDocument document)
        {
            base.ImportFromXml(document);
            Brand = document.GetValue("/pod/data/object/technical_metadata/tape_stock_brand", string.Empty);
            DirectionsRecorded = document.GetValue("/pod/data/object/technical_metadata/directions_recorded", string.Empty);
            PlaybackSpeed = document.BoolValuesToString("/pod/data/object/technical_metadata/playback_speed", string.Empty);
            TrackConfiguration = document.BoolValuesToString("/pod/data/object/technical_metadata/track_configuration", string.Empty);
            SoundField = document.BoolValuesToString("/pod/data/object/technical_metadata/sound_field", string.Empty);
            TapeThickness = document.BoolValuesToString("/pod/data/object/technical_metadata/tape_thickness", string.Empty);

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