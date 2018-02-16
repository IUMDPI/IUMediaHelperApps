using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    public interface IMediaFormat
    {
        string Key { get; }
        string ProperName { get; }
    }

    public class MediaFormat : IMediaFormat
    {
        internal MediaFormat(string key, string properName)
        {
            Key = key;
            ProperName = properName;
        }

        public string Key { get; }
        public string ProperName { get; }
    }

    public class UnknownMediaFormat : IMediaFormat
    {
        public UnknownMediaFormat(string properName)
        {
            ProperName = properName;
        }

        public string Key => "";
        public string ProperName { get; }
    }


    public static class MediaFormats
    {
        public static readonly IMediaFormat AudioCassette = new MediaFormat("audiocassette", "Audio Cassette");
        public static readonly IMediaFormat OpenReelAudioTape = new MediaFormat("open reel audio tape", "Open Reel Audio Tape");
        public static readonly IMediaFormat Lp = new MediaFormat("lp", "Lp");
        public static readonly IMediaFormat Cdr = new MediaFormat("cd-r", "CD Rom");
        public static readonly IMediaFormat FortyFive = new MediaFormat("45", "45");
        public static readonly IMediaFormat LacquerDisc = new MediaFormat("lacquer disc", "Lacquer Disc");
        public static readonly IMediaFormat Cylinder = new MediaFormat("cylinder", "Cylinder");
        public static readonly IMediaFormat SeventyEight = new MediaFormat("78", "78");
        public static readonly IMediaFormat Vhs = new MediaFormat("vhs", "VHS");
        public static readonly IMediaFormat Betacam = new MediaFormat("betacam", "Betacam");
        public static readonly IMediaFormat BetacamAnamorphic = new MediaFormat("betacam:Anamorphic", "Betacam: Anamorphic");
        public static readonly IMediaFormat Dat = new MediaFormat("dat", "DAT");
        public static readonly IMediaFormat OneInchOpenReelVideoTape = new MediaFormat("1-inch open reel video tape", "1-inch Open Reel Video Tape");
        public static readonly IMediaFormat EightMillimeterVideo = new MediaFormat("8mm video", "8mm Video");
        public static readonly IMediaFormat EightMillimeterVideoQuadaudio = new MediaFormat("8mm video:quadaudio", "8mm Video: Quadaudio");
        public static readonly IMediaFormat Umatic = new MediaFormat("u-matic", "U-matic");
        public static readonly IMediaFormat Betamax = new MediaFormat("betamax", "Betamax");
        public static readonly IMediaFormat AluminumDisc = new MediaFormat("aluminum disc", "Aluminum Disc");
        public static readonly IMediaFormat OtherAnalogSoundDisc = new MediaFormat("other analog sound disc", "Other Analog Sound Disc");
        
        private static readonly List<IMediaFormat> AllKnownFormats = new List<IMediaFormat>
        {
            AudioCassette,
            OpenReelAudioTape,
            Lp,
            Cdr,
            FortyFive,
            LacquerDisc,
            Cylinder,
            SeventyEight,
            Vhs,
            Betacam,
            BetacamAnamorphic,
            Dat,
            OneInchOpenReelVideoTape,
            EightMillimeterVideo,
            EightMillimeterVideoQuadaudio,
            Umatic,
            Betamax,
            AluminumDisc,
            OtherAnalogSoundDisc
        };

        public static IMediaFormat GetFormat(string key)
        {
            var format = AllKnownFormats.SingleOrDefault(
                f => f.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            return format ?? new UnknownMediaFormat(key);
        }

    }
}
