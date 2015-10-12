using System;
using System.IO;

namespace WaveInfo
{
    public class InfoChunk : AbstractChunk
    {
        public string Text { get; set; }

        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            Text = new string(reader.ReadChars(Convert.ToInt32(Size)));
        }
    }
}