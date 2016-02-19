using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using Packager.Models.OutputModels.Ingest;

namespace Packager.Test.Models.OutputModels
{
    [TestFixture]
    public abstract class AbstractIngestModelTests
    {
        private static bool OrderAttributePresent(MemberInfo info, int order)
        {
            var attribute = info.GetCustomAttribute<XmlElementAttribute>();
            return attribute?.Order == order;
        }

        private static bool ElementNamePresent(MemberInfo info, string name)
        {
            var attribute = info.GetCustomAttribute<XmlElementAttribute>();
            return attribute?.ElementName == name;
        }

        public class VideoIngestTests : AbstractIngestModelTests
        {
            [TestCase("Date", 1)]
            [TestCase("DigitStatus", 2)]
            [TestCase("Comments", 3)]
            [TestCase("CreatedBy", 4)]
            [TestCase("Players", 5)]
            [TestCase("TbcDevices", 6)]
            [TestCase("AdDevices", 7)]
            [TestCase("ExtractionWorkstation", 8)]
            [TestCase("Encoder", 9)]
            public void FieldsShouldHaveCorrectOrderAttributes(string field, int order)
            {
                var info = typeof (VideoIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(OrderAttributePresent(info, order), Is.True);
            }
            
            [TestCase("CreatedBy", "Created_by")]
            [TestCase("Players", "Player")]
            [TestCase("AdDevices", "AD")]
            [TestCase("TbcDevices", "TBC")]
            [TestCase("ExtractionWorkstation", "Extraction_workstation")]
            public void FieldsShouldHaveCorrectElementNameAttribute(string field, string name)
            {
                var info = typeof(VideoIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(ElementNamePresent(info, name), Is.True);
            }
            
        }

        public class AudioIngestTests : AbstractIngestModelTests
        {
            [TestCase("Date", 1)]
            [TestCase("Comments", 2)]
            [TestCase("CreatedBy", 3)]
            [TestCase("SpeedUsed", 4)]
            [TestCase("Stylus", 5)]
            [TestCase("Turnover", 6)]
            [TestCase("Rolloff", 7)]
            [TestCase("ReferenceFluxivity", 8)]
            [TestCase("Gain", 9)]
            [TestCase("AnalogOutputVoltage", 10)]
            [TestCase("Peak", 11)]
            [TestCase("Players", 12)]
            [TestCase("AdDevices", 13)]
            [TestCase("PreAmpDevices", 14)]
            [TestCase("ExtractionWorkstation", 15)]
            public void FieldsShouldHaveCorrectOrderAttributes(string field, int order)
            {
                var info = typeof (AudioIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(OrderAttributePresent(info, order), Is.True);
            }
            
            [TestCase("CreatedBy", "Created_by")]
            [TestCase("SpeedUsed", "Speed_used")]
            [TestCase("Players", "Player")]
            [TestCase("AdDevices", "AD")]
            [TestCase("PreAmpDevices", "PreAmp")]
            [TestCase("ReferenceFluxivity", "Reference_fluxivity")]
            [TestCase("AnalogOutputVoltage", "Analog_output_voltage")]
            [TestCase("ExtractionWorkstation", "Extraction_workstation")]
            public void FieldsShouldHaveCorrectElementNameAttribute(string field, string name)
            {
                var info = typeof (AudioIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(ElementNamePresent(info, name), Is.True);
            }
        }
    }
}