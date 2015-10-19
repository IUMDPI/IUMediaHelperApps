using System;
using System.IO;
using System.Text;
using WaveInfo.Extensions;

namespace WaveInfo
{
    public class BextChunk : AbstractChunk
    {
        public string Description { get; set; } // max 256 chars
        public string Originator { get; set; } // max 32 chars
        public string OriginatorReference { get; set; } // max 32 chars
        public string OriginationDate { get; set; }
        public string OriginationTime { get; set; }
        public long TimeReference { get; set; } // first sample count since midnight
        public short Version { get; set; } // version 2 has loudness stuff which we don't know so using version 1
        public string UniqueMaterialIdentifier { get; set; } // 64 bytes http://en.wikipedia.org/wiki/UMID
        public byte[] Reserved { get; set; }
        public string CodingHistory { get; set; }

        public override void ReadChunk(BinaryReader reader)
        {
            base.ReadChunk(reader);

            Description = new string(reader.ReadChars(256)).AppendEnd();
            Originator = new string(reader.ReadChars(32)).AppendEnd();
            OriginatorReference = new string(reader.ReadChars(32)).AppendEnd();
            ;
            OriginationDate = new string(reader.ReadChars(10)).AppendEnd();
            OriginationTime = new string(reader.ReadChars(8)).AppendEnd();
            TimeReference = reader.ReadInt64();
            Version = reader.ReadInt16();
            UniqueMaterialIdentifier = new string(reader.ReadChars(64)).AppendEnd();
            Reserved = reader.ReadBytes(190);

            if (Size > 602)
            {
                var historySize = Convert.ToInt32((Size - 602).Normalize());
                CodingHistory = new string(reader.ReadChars(historySize)).AppendEnd();
            }
        }

        public override string GetReport()
        {
            var builder = new StringBuilder(base.GetReport());
            builder.AppendLine(this.ToColumns("Description"));
            builder.AppendLine(this.ToColumns("Originator"));
            builder.AppendLine(this.ToColumns("OriginatorReference"));
            builder.AppendLine(this.ToColumns("OriginationDate"));
            builder.AppendLine(this.ToColumns("OriginationTime"));
            builder.AppendLine(this.ToColumns("TimeReference"));
            builder.AppendLine(this.ToColumns("Version"));
            builder.AppendLine("Umid".ToColumns(UniqueMaterialIdentifier));
            builder.AppendLine("Reserved".ToColumns($"Byte Array [{Reserved.Length}]"));
            builder.AppendLine(this.ToColumns("CodingHistory"));

            return builder.ToString();
        }
    }
}