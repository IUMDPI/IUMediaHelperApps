using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Models
{
    public abstract class OperationReport
    {
        [XmlAttribute("Timestamp")]
        public DateTime Timestamp { get; set; }
        public bool Succeeded { get; set; }
        public string Issue { get; set; }

        public void Save(string folderPath)
        {
            try
            {
                var filename = Path.Combine(folderPath, $"Packager_{DateTime.Now.Ticks:D19}.xml");

                var serializer = new XmlSerializer(GetType());
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, this);
                    writer.Close();
                }
            }
            catch(Exception)
            {
                // ignore   
            }
        }

        public static Task<T> Read<T>(string path) where T:OperationReport
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

    
    public class PackagerObjectReport : OperationReport
    {
        [XmlAttribute("Barcode")]
        public string Barcode { get; set; }
    }

    public class PackagerReport : OperationReport
    {
        [XmlArray("Objects")]
        [XmlArrayItem("Object")]
        public List<PackagerObjectReport> ObjectReports { get; set; }
    }
}
