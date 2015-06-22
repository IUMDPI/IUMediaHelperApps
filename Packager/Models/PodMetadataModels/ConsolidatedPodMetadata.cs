using System.Collections.Generic;
using Packager.Attributes;
using Packager.Observers;
using Packager.Providers;
using RestSharp.Deserializers;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {
        [DeserializeAs(Name = "success")]
        public bool Success { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }

        [DeserializeAs(Name = "call_number")]
        public string CallNumber { get; set; }

        [DeserializeAs(Name = "title")]
        public string Title { get; set; }

        [DeserializeAsLookup(Name = "unit", LookupTable = LookupTables.Units)]
        public string Unit { get; set; }

        [DeserializeAs(Name = "mdpi_barcode")]
        public string Barcode { get; set; }

        [DeserializeAs(Name = "tape_stock_brand")]
        public string Brand { get; set; }

        [DeserializeAs(Name = "id")]
        public string Identifier { get; set; }

        [DeserializeAs(Name = "format")]
        public string CarrierType { get; set; }

        [DeserializeAs(Name = "directions_recorded")]
        public string DirectionsRecorded { get; set; }

        [DeserializeAsLookup(Name = "playback_speed", LookupTable = LookupTables.PlaybackSpeeds)]
        public string[] PlaybackSpeeds { get; set; }

        [DeserializeAsLookup(Name = "track_configuration", LookupTable = LookupTables.TrackConfigurations)]
        public string[] TrackConfigurations { get; set; }

        [DeserializeAsLookup(Name = "sound_field", LookupTable = LookupTables.SoundFields)]
        public string[] SoundFields { get; set; }

        [DeserializeAsLookup(Name = "tape_thickness", LookupTable = LookupTables.TapeThicknesses)]
        public string[] TapeThicknesses { get; set; }

    }
}