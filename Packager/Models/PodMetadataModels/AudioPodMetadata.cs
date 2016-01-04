using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class AudioPodMetadata : AbstractPodMetadata
    {
        private const string LacquerDiscFormatIdentifier = "lacquer disc";
        private const string OpenReelFormatIdentifier = "open reel audio tape";

        public string Brand { get; set; }

        public string DirectionsRecorded { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }

        public override void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            base.ImportFromXml(element, lookupsProvider);
            Brand = element.ToStringValue("data/object/technical_metadata/tape_stock_brand");
            DirectionsRecorded = element.ToStringValue("data/object/technical_metadata/directions_recorded");
            PlaybackSpeed = GetPlaybackSpeedForFormat(element, lookupsProvider);
            TrackConfiguration = element.ToResolvedDelimitedString("data/object/technical_metadata/track_configuration", lookupsProvider.TrackConfiguration);
            SoundField = GetSoundFieldForFormat(element, lookupsProvider); 
            TapeThickness = element.ToResolvedDelimitedString("data/object/technical_metadata/tape_thickness", lookupsProvider.TapeThickness);
        }

        private string GetPlaybackSpeedForFormat(XElement element, ILookupsProvider lookupsProvider)
        {
            switch (Format.ToLowerInvariant())
            {
                case LacquerDiscFormatIdentifier:
                    return element.ToStringValue("data/object/technical_metadata/speed").AppendIfValuePresent(" rpm");
                case OpenReelFormatIdentifier:
                    return element.ToResolvedDelimitedString("data/object/technical_metadata/playback_speed", lookupsProvider.PlaybackSpeed);
                default:
                    throw new PodMetadataException("unknown format value: {0}", Format);
            }
        }
        private string GetSoundFieldForFormat(XElement element, ILookupsProvider lookupsProvider)
        {
            switch (Format.ToLowerInvariant())
            {
                case LacquerDiscFormatIdentifier:
                    return element.ToStringValue("data/object/technical_metadata/sound_field");
                case OpenReelFormatIdentifier:
                    return element.ToResolvedDelimitedString("data/object/technical_metadata/sound_field", lookupsProvider.PlaybackSpeed);
                default:
                    throw new PodMetadataException("unknown format value: {0}", Format);
            }
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements, ILookupsProvider lookupsProvider)
        {
            return elements.Select(element => element.ToImportable<DigitalAudioFile>(lookupsProvider))
                .Cast<AbstractDigitalFile>().ToList();
        }

        protected override void NormalizeFileProvenances()
        {
            if (Format.Equals(LacquerDiscFormatIdentifier, StringComparison.InvariantCultureIgnoreCase)==false)
            {
                return;
            }

            foreach (var provenance in FileProvenances.Select(p=>p as DigitalAudioFile))
            {
                provenance.SpeedUsed = provenance.SpeedUsed.AppendIfValuePresent(" rpm");
            }
        }
    }
}