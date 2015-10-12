using System.IO;
using System.Text;

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
            Size = NormalizeSize(ReportedSize);
        }

        private void SetOffset(BinaryReader reader)
        {
            Offset = reader.BaseStream.Position - 4;
        }

        protected static uint NormalizeSize(uint value)
        {
            if (value%2 == 0)
            {
                return value;
            }

            return value + 1;
        }

        public string GetReport()
        {
            var builder = new StringBuilder();

            foreach (var property in GetType().GetProperties())
            {
                var lineFormat = "{0,-20}{1}";
                var line = property.PropertyType.IsArray
                    ? string.Format(lineFormat, property.Name, "[array]")
                    : string.Format(lineFormat, property.Name, property.GetValue(this));
                builder.AppendLine(line);
            }

            return builder.ToString();
        }
    }
}