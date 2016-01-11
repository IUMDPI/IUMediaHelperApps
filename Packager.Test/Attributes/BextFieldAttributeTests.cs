using NUnit.Framework;
using Packager.Attributes;
using Packager.Utilities.Bext;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class BextFieldAttributeTests
    {
        [TestCase("value", 5, true)]
        [TestCase("value", 4, false)]
        [TestCase("value", 0, true)]
        [TestCase("", 5, true)]
        public void ValueWithinLengthLimitShouldReturnCorrectResults(string value, int maxLength, bool expected)
        {
            var attribute = new BextFieldAttribute(BextFields.Description, maxLength);

            Assert.That(attribute.ValueWithinLengthLimit(value), Is.EqualTo(expected));
        }

        [TestCase(BextFields.Description, "description")]
        [TestCase(BextFields.Originator, "originator")]
        [TestCase(BextFields.OriginatorReference, "originator_reference")]
        [TestCase(BextFields.OriginationDate, "origination_date")]
        [TestCase(BextFields.OriginationTime, "origination_time")]
        [TestCase(BextFields.TimeReference, "time_reference")]
        [TestCase(BextFields.UMID, "umid")]
        [TestCase(BextFields.IARL, "IARL")]
        [TestCase(BextFields.IART, "IART")]
        [TestCase(BextFields.ICMS, "ICMS")]
        [TestCase(BextFields.ICMT, "ICMT")]
        [TestCase(BextFields.ICOP, "ICOP")]
        [TestCase(BextFields.ICRD, "ICRD")]
        [TestCase(BextFields.IENG, "IENG")]
        [TestCase(BextFields.IGNR, "IGNR")]
        [TestCase(BextFields.IKEY, "IKEY")]
        [TestCase(BextFields.IMED, "IMED")]
        [TestCase(BextFields.INAM, "INAM")]
        [TestCase(BextFields.IPRD, "IPRD")]
        [TestCase(BextFields.ISBJ, "ISBJ")]
        [TestCase(BextFields.ISFT, "ISFT")]
        [TestCase(BextFields.ISRC, "ISRC")]
        [TestCase(BextFields.ISRF, "ISRF")]
        [TestCase(BextFields.ITCH, "ITCH")]
        public void GetFFMPEGArgumentShouldReturnCorrectResult(BextFields field, string expected)
        {
            var attribute = new BextFieldAttribute(field);
            Assert.That(attribute.GetFFMPEGArgument(), Is.EqualTo(expected));
        }

        [Test]
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new BextFieldAttribute(BextFields.History, 12);
            Assert.That(attribute.Field, Is.EqualTo(BextFields.History));
            Assert.That(attribute.MaxLength, Is.EqualTo(12));
        }

        [Test]
        public void MaxLengthShouldBeZeroWhenNotSpecifiedInConstructor()
        {
            var attribute = new BextFieldAttribute(BextFields.History);
            Assert.That(attribute.Field, Is.EqualTo(BextFields.History));
            Assert.That(attribute.MaxLength, Is.EqualTo(0));
        }
    }
}