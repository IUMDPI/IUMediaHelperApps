using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    public class Formats
    {
        private Formats(string key, string properName)
        {
            Key = key;
            ProperName = properName;
        }

        public string Key { get; }
        public string ProperName { get; }

        public static Formats AudioCassette = new Formats("audiocassette", "Audio Cassette");
        public static Formats OpenReelAudioTape = new Formats("open reel audio tape", "Open Reel Audio Tape");
        public static Formats Lp = new Formats("lp", "Lp");
        public static Formats Cdr = new Formats("cd-r", "CD Rom");
        public static Formats FortyFive = new Formats("45", "45");
        public static Formats LacquerDisc = new Formats("lacquer disc", "Lacquer Disc");
        public static Formats Cylinder = new Formats("cylinder", "Cylinder");
        public static Formats SeventyEight = new Formats("78", "78");
        public static Formats Vhs = new Formats("vhs", "VHS");
        public static Formats Betacam = new Formats("betacam", "Betacam");
        public static Formats BetacamAnamorphic = new Formats("betacam:Anamorphic", "Betacam: Anamorphic");
        public static Formats Dat = new Formats("dat", "DAT");
        public static Formats OneInchOpenReelVideoTape = new Formats("1-inch open reel video tape", "1-inch Open Reel Video Tape");
        public static Formats EightMillimeterVideo = new Formats("8mm video", "8mm Video");
        public static Formats EightMillimeterVideoQuadaudio = new Formats("8mm video:quadaudio", "8mm Video: Quadaudio");
        public static Formats Umatic = new Formats("u-matic", "U-matic");
        public static Formats Betamax = new Formats("betamax", "Betamax");
        public static Formats UnknownFormat = new Formats("", "Unknown/Unsupported Format");

        private static readonly List<Formats> AllSupportedFormats = new List<Formats>
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

        public static Formats GetFormat(string key)
        {
            var format = AllSupportedFormats.SingleOrDefault(
                f => f.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            return format ?? UnknownFormat;
        }

    }
}
