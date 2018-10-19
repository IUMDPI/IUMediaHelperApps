using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Exceptions;
using Packager.Factories;
using RestSharp.Extensions;

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

        public string TapeType { get; set; }
        public string TapeStockBrand { get; set; }
        public string NoiseReduction { get; set; }
        public string FormatDuration { get; set; }

        private static readonly Dictionary<string, string> KnownTapeTypes = new Dictionary<string, string>
        {
            {"i", "Type I"},
            {"ii", "Type II"},
            {"iii", "Type III"},
            {"iv", "Type IV"},
            {"unknown", "Unknown"}
        };

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
            TapeType = ConvertTapeType(factory.ToStringValue(element, "data/tape_type"));
            TapeStockBrand = factory.ToStringValue(element, "data/tape_stock_brand");
            NoiseReduction = factory.ToStringValue(element, "data/noise_reduction");
            FormatDuration = factory.ToStringValue(element, "data/format_duration");
        }

        private static string ConvertTapeType(string value)
        {
            if (!value.HasValue())
            {
                return value;
            }

            if (!KnownTapeTypes.ContainsKey(value.ToLowerInvariant()))
            {
                throw new PodMetadataException($"{value} does not correspond to a known tape type");
            }

            return KnownTapeTypes[value.ToLowerInvariant()];
        }
        
        protected override List<AbstractDigitalFile> ImportFileProvenances(XElement element, string path, 
            IImportableFactory factory)
        {
            return factory.ToObjectList<DigitalAudioFile>(element, path)
                .Cast<AbstractDigitalFile>().ToList();
        }
    }
}