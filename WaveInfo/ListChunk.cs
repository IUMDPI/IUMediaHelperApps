using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaveInfo
{
    public class ListChunk:AbstractChunk
    {
        public string ListType;
        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            ListType = new string(reader.ReadChars(4));
        }
    }
}
