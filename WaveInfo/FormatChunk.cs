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
        public ushort ExtensionSize;
        public ushort ValidBitsPerSample;
        public uint ChannelMask;
        public byte[] SubFormat;
        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            FormatTag = reader.ReadUInt16();
            Channels = reader.ReadUInt16();
            SamplesPerSec = reader.ReadUInt32();
            AvgBytesPerSec = reader.ReadUInt32();
            BlockAlign = reader.ReadUInt16();
            BitsPerSample = reader.ReadUInt16();

            if (Size == 16)
            {
                return;
            }

            ExtensionSize = reader.ReadUInt16();
            if (ExtensionSize == 0)
            {
                return;
            }

            ValidBitsPerSample = reader.ReadUInt16();
            ChannelMask = reader.ReadUInt32();
            SubFormat = reader.ReadBytes(16);
        }
    }
}