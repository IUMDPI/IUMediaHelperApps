using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using Packager.Models.OutputModels.Carrier;

namespace Packager.Test.Models.OutputModels.CarrierModelTests
{
    [TestFixture]
    public abstract class AbstractCarrierModelTests
    {
        private static bool OrderAttributePresent(MemberInfo info, int order)
        {
            var attribute = info.GetCustomAttribute<XmlElementAttribute>();
            return attribute?.Order == order;
        }

        private static bool XmlAttributeAttributePresent(MemberInfo info, string attributeName)
        {
            var attribute = info.GetCustomAttribute<XmlAttributeAttribute>();
            return attribute?.AttributeName == attributeName;
        }

        public class VideoCarrierTests : AbstractCarrierModelTests
        {
            [TestCase("Identifier", 1)]
            [TestCase("Barcode", 2)]
            [TestCase("PhysicalCondition", 3)]
            [TestCase("Configuration", 4)]
            [TestCase("Parts", 5)]
            [TestCase("RecordingStandard", 6)]
            [TestCase("ImageFormat", 7)]
            [TestCase("Definition", 8)]
            [TestCase("Preview", 9)]
            [TestCase("Comments", 10)]
            [TestCase("Cleaning", 11)]
            [TestCase("Baking", 12)]
            public void FieldsShouldHaveCorrectOrderAttributes(string field, int order)
            {
                var info = typeof (VideoCarrier).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(OrderAttributePresent(info, order), Is.True);
            }

            [Test]
            public void CarrierTypeShouldHaveCorrectAttributeAttribute()
            {
                var info = typeof (VideoCarrier).GetProperty("CarrierType");
                Assert.That(info, Is.Not.Null);
                Assert.That(XmlAttributeAttributePresent(info, "type"), Is.True);
            }
        }

        public class AudioCarrierTests : AbstractCarrierModelTests
        {
            [TestCase("Identifier", 1)]
            [TestCase("Barcode", 2)]
            [TestCase("PhysicalCondition", 3)]
            [TestCase("Configuration", 4)]
            [TestCase("Brand", 5)]
            [TestCase("Thickness", 6)]
            [TestCase("DirectionsRecorded", 7)]
            [TestCase("Repaired", 8)]
            [TestCase("Parts", 9)]
            [TestCase("Preview", 10)]
            [TestCase("Comments", 11)]
            [TestCase("Cleaning", 12)]
            [TestCase("Baking", 13)]
            public void FieldsShouldHaveCorrectOrderAttributes(string field, int order)
            {
                var info = typeof (AudioCarrier).GetProperty(field);
                Assert.That(info, Is.Not.Null);
                Assert.That(OrderAttributePresent(info, order), Is.True);
            }

            [Test]
            public void CarrierTypeShouldHaveCorrectAttributeAttribute()
            {
                var info = typeof (AudioCarrier).GetProperty("CarrierType");
                Assert.That(info, Is.Not.Null);
                Assert.That(XmlAttributeAttributePresent(info, "type"), Is.True);
            }
        }
    }
}