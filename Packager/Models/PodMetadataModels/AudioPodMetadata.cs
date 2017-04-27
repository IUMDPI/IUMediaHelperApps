using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Factories;

namespace Packager.Models.PodMetadataModels
{
    public class AudioPodMetadata : AbstractPodMetadata
    {
        public string Brand { get; set; }
        public string DirectionsRecorded { get; set; }
     
        public string TrackConfiguration { get; set; }
        public string SoundField { get; set; }
        public string TapeThickness { get; set; }
        public string TapeBase { get; set; }

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);
            Brand = factory.ToStringValue(element,"data/tape_stock_brand");
            DirectionsRecorded = factory.ToStringValue(element,"data/directions_recorded");
            PlaybackSpeed = factory.ToStringValue(element, "data/playback_speed");
            TrackConfiguration = factory.ToStringValue(element, "data/track_configuration");
            SoundField = factory.ToStringValue(element, "data/sound_field");
            TapeThickness = factory.ToStringValue(element,"data/tape_thickness");
            TapeBase = factory.ToStringValue(element, "data/tape_base");
        }
        
        protected override List<AbstractDigitalFile> ImportFileProvenances(XElement element, string path, 
            IImportableFactory factory)
        {
            return factory.ToObjectList<DigitalAudioFile>(element, path)
                .Cast<AbstractDigitalFile>().ToList();
        }
    }
}