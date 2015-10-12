using System.IO;

namespace WaveInfo
{
    public class FactChunk:AbstractChunk
    {
        public uint Samples; //Number of audio frames;
        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            Samples = reader.ReadUInt32();
        }
    }
}