using System.IO;

namespace WaveInfo
{
    public class FormatChunk : AbstractChunk
    {
        public uint AvgBytesPerSec;
        public ushort BitsPerSample;
        public ushort BlockAlign;
        public ushort Channels;
        public ushort FormatTag;
        public uint SamplesPerSec;
        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            FormatTag = reader.ReadUInt16();
            Channels = reader.ReadUInt16();
            SamplesPerSec = reader.ReadUInt32();
            AvgBytesPerSec = reader.ReadUInt32();
            BlockAlign = reader.ReadUInt16();
            BitsPerSample = reader.ReadUInt16();
        }
    }
}