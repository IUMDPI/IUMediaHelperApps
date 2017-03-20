using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Models
{
    public abstract class AbstractOperationReport
    {
        private TimeSpan _duration;

        [XmlAttribute("Timestamp")]
        public DateTime Timestamp { get; set; }
        public bool Succeeded { get; set; }
        public string Issue { get; set; }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get {return _duration;}
            set { _duration = value; }
        }

        [XmlAttribute("Duration")]
        public long DurationTicks
        {
            get { return _duration.Ticks; }
            set { _duration = new TimeSpan(value);}
        }

        public static Task<T> Read<T>(string path) where T:AbstractOperationReport
        {
            return Task.Run(() =>
            {
                using (var inputStream = File.Open(path, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var reader = XmlReader.Create(inputStream);
                    return (T)serializer.Deserialize(reader);
                }
            });
        }
    }
}