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

            reader.BaseStream.Seek(Size, SeekOrigin.Current);
            //Data = reader.ReadBytes(Convert.ToInt32(Size));
        }
    }
}