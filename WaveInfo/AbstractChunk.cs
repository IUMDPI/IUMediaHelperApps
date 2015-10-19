using System.IO;
using System.Text;
using WaveInfo.Extensions;

namespace WaveInfo
{
    public abstract class AbstractChunk
    {
        public string Id { get; set; }
        public uint Size { get; set; }
        public uint ReportedSize { get; set; }
        public long Offset { get; set; }

        public virtual void ReadChunk(BinaryReader reader)
        {
            SetOffset(reader);
            SetSize(reader);
        }

        private void SetSize(BinaryReader reader)
        {
            ReportedSize = reader.ReadUInt32();
            Size = ReportedSize.Normalize();
        }

        private void SetOffset(BinaryReader reader)
        {
            Offset = reader.BaseStream.Position - 4;
        }

        
        public virtual string GetReport()
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine($"{Id} chunk");
            builder.AppendLine(new string('-', 75));
            builder.AppendLine(this.ToColumns("Size"));
            builder.AppendLine(this.ToColumns("ReportedSize"));
            builder.AppendLine(this.ToColumns("Offset"));

            return builder.ToString();
        }

        
    }
}