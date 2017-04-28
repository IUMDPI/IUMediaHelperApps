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

    public class MediaFormats:IMediaFormat
    {
        private MediaFormats(string key, string properName)
        {
            Key = key;
            ProperName = properName;
        }

        public string Key { get; }
        public string ProperName { get; }

        public static MediaFormats AudioCassette = new MediaFormats("audiocassette", "Audio Cassette");
        public static MediaFormats OpenReelAudioTape = new MediaFormats("open reel audio tape", "Open Reel Audio Tape");
        public static MediaFormats Lp = new MediaFormats("lp", "Lp");
        public static MediaFormats Cdr = new MediaFormats("cd-r", "CD Rom");
        public static MediaFormats FortyFive = new MediaFormats("45", "45");
        public static MediaFormats LacquerDisc = new MediaFormats("lacquer disc", "Lacquer Disc");
        public static MediaFormats Cylinder = new MediaFormats("cylinder", "Cylinder");
        public static MediaFormats SeventyEight = new MediaFormats("78", "78");
        public static MediaFormats Vhs = new MediaFormats("vhs", "VHS");
        public static MediaFormats Betacam = new MediaFormats("betacam", "Betacam");
        public static MediaFormats BetacamAnamorphic = new MediaFormats("betacam:Anamorphic", "Betacam: Anamorphic");
        public static MediaFormats Dat = new MediaFormats("dat", "DAT");
        public static MediaFormats OneInchOpenReelVideoTape = new MediaFormats("1-inch open reel video tape", "1-inch Open Reel Video Tape");
        public static MediaFormats EightMillimeterVideo = new MediaFormats("8mm video", "8mm Video");
        public static MediaFormats EightMillimeterVideoQuadaudio = new MediaFormats("8mm video:quadaudio", "8mm Video: Quadaudio");
        public static MediaFormats Umatic = new MediaFormats("u-matic", "U-matic");
        public static MediaFormats Betamax = new MediaFormats("betamax", "Betamax");
        public static MediaFormats UnknownMediaFormat = new MediaFormats("", "Unknown Format");

        private static readonly List<MediaFormats> AllKnownFormats = new List<MediaFormats>
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
            Betamax
        };

        public static MediaFormats GetFormat(string key)
        {
            var format = AllKnownFormats.SingleOrDefault(
                f => f.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            return format ?? UnknownMediaFormat;
        }

    }
}
