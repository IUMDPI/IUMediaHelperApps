using System.IO;

namespace WaveInfo
{
    public class Ds64Chunk : AbstractChunk
    {
        public ulong RiffSize { get; set; }
        public ulong DataSize { get; set; }
        public ulong SampleCount { get; set; }
        public uint TableLength { get; set; }

        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            RiffSize = reader.ReadUInt64();
            DataSize = reader.ReadUInt64();
            SampleCount = reader.ReadUInt64();
            TableLength = reader.ReadUInt32();
        }
    }
}