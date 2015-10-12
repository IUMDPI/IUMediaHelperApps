using System;
using System.IO;

namespace WaveInfo
{
    public class OtherChunk : AbstractChunk
    {
        public byte[] Data { get; set; }
        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            Data = reader.ReadBytes(Convert.ToInt32(Size));
        }
    }
}