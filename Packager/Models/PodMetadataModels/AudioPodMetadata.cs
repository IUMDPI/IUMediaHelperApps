using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
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

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);
            Brand = factory.ToStringValue(element,"data/object/technical_metadata/tape_stock_brand");
            DirectionsRecorded = factory.ToStringValue(element,"data/object/technical_metadata/directions_recorded");
            PlaybackSpeed = GetPlaybackSpeedForFormat(element, factory);
            TrackConfiguration = factory.ResolveTrackConfiguration(element, "data/object/technical_metadata/track_configuration");
            SoundField = GetSoundFieldForFormat(element, factory);
            TapeThickness = factory.ResolveTapeThickness(element,"data/object/technical_metadata/tape_thickness");
        }

        private string GetPlaybackSpeedForFormat(XElement element, IImportableFactory factory)
        {
            switch (Format.ToLowerInvariant())
            {
                case LacquerDiscFormatIdentifier:
                    return factory.ToStringValue(element,"data/object/technical_metadata/speed", " rpm");
                case OpenReelFormatIdentifier:
                    return factory.ResolvePlaybackSpeed(element, "data/object/technical_metadata/playback_speed");
                default:
                    throw new PodMetadataException("unknown format value: {0}", Format);
            }
        }

        private string GetSoundFieldForFormat(XElement element, IImportableFactory factory)
        {
            switch (Format.ToLowerInvariant())
            {
                case LacquerDiscFormatIdentifier:
                    return factory.ToStringValue(element,"data/object/technical_metadata/sound_field");
                case OpenReelFormatIdentifier:
                    return factory.ResolveSoundField(element, "data/object/technical_metadata/sound_field");
                default:
                    throw new PodMetadataException("unknown format value: {0}", Format);
            }
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements,
            IImportableFactory factory)
        {
            return elements.Select(factory.ToImportable<DigitalAudioFile>)
                .Cast<AbstractDigitalFile>().ToList();
        }

        protected override void NormalizeFileProvenances()
        {
            if (Format.Equals(LacquerDiscFormatIdentifier, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                return;
            }

            foreach (var provenance in FileProvenances.Select(p => p as DigitalAudioFile))
            {
                provenance.SpeedUsed = provenance.SpeedUsed.AppendIfValuePresent(" rpm");
            }
        }
    }
}