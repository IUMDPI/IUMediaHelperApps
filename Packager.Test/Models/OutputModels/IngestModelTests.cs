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

            [Test]
            public void CreatedByShouldHaveCorrectElementNameAttribute()
            {
                var info = typeof (VideoIngest).GetProperty("CreatedBy");
                Assert.That(info, Is.Not.Null);
                Assert.That(ElementNamePresent(info, "Created_by"), Is.True);
            }
        }

        public class AudioIngestTests : AbstractIngestModelTests
        {
            [TestCase("Date", 1)]
            [TestCase("Comments", 2)]
            [TestCase("CreatedBy", 3)]
            [TestCase("SpeedUsed", 4)]
            [TestCase("PreAmp", 5)]
            [TestCase("PreAmpSerialNumber", 6)]
            [TestCase("Stylus", 7)]
            [TestCase("Turnover", 8)]
            [TestCase("Rolloff", 9)]
            [TestCase("ReferenceFluxivity", 10)]
            [TestCase("Gain", 11)]
            [TestCase("AnalogOutputVoltage", 12)]
            [TestCase("Peak", 13)]
            [TestCase("Players", 14)]
            [TestCase("AdDevices", 15)]
            [TestCase("ExtractionWorkstation", 16)]
            public void FieldsShouldHaveCorrectOrderAttributes(string field, int order)
            {
                var info = typeof (AudioIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(OrderAttributePresent(info, order), Is.True);
            }


            [TestCase("CreatedBy", "Created_by")]
            [TestCase("SpeedUsed", "Speed_used")]
            [TestCase("PreAmp", "Preamp")]
            [TestCase("PreAmpSerialNumber", "Preamp_serial_number")]
            [TestCase("ReferenceFluxivity", "Reference_fluxivity")]
            [TestCase("AnalogOutputVoltage", "Analog_output_voltage")]
            public void FieldsShouldHaveCorrectElementNameAttribute(string field, string name)
            {
                var info = typeof (AudioIngest).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(ElementNamePresent(info, name), Is.True);
            }
        }
    }
}