using System.IO;

namespace WaveInfo
{
    public class RiffChunk : AbstractChunk
    {
        public uint FileLength; //In bytes, measured from offset 8
        public string RiffType; //WAVE, usually

        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            RiffType = new string(reader.ReadChars(4));
        }
    }
}