using System;
using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class StringExtensionTests
    {
        [TestCase(@"testing """, @"testing \""")]
        public void NormalizeForCommandLineShouldEscapeQuotesCorrectly(string original, string expected)
        {
            var result = original.NormalizeForCommandLine();
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToQuotedShouldReturnCorrectValue()
        {
            Assert.That("testing".ToQuoted(), Is.EqualTo("\"testing\""));
        }

        [TestCase("", "default")]
        [TestCase(null, "default")]
        [TestCase("value", "value")]
        public void ToDefaultIfEmptyShouldReturnCorrectValues(string value, string expected)
        {
            Assert.That(value.ToDefaultIfEmpty("default"), Is.EqualTo(expected));
        }

        [TestCase(" test", "test")]
        [TestCase("test ", "test")]
        [TestCase(" test ", "test")]
        [TestCase("test test", "testtest")]
        [TestCase("","")]
        [TestCase(null,null)]
        public void RemoveSpacesShouldReturnCorrectValues(string value, string expected)
        {
            Assert.That(value.RemoveSpaces(), Is.EqualTo(expected));
        }

        [TestCase(" test", "test")]
        [TestCase("test ", "test")]
        [TestCase(" test ", "test")]
        [TestCase("test test", "test test")]
        [TestCase(" test test ", "test test")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void TrimWhiteSpaceShouldReturnCorrectValues(string value, string expected)
        {
            Assert.That(value.TrimWhiteSpace(), Is.EqualTo(expected));
        }

        private class TestClass
        {
            public string StringFieldOne { get; set; }
            public string StringFieldTwo { get; set; }
            public DateTime? DateFieldOne { get; set; }
            public DateTime? DateFieldTwo { get; set; }
        }

        [TestCase("set", "set")]
        [TestCase("set", "")]
        [TestCase("", "set")]
        [TestCase("", "")]
        public void GetStringPropertiesAndValuesShouldReturnCorrectResult(string field1, string field2)
        {
            var instance = new TestClass {StringFieldOne = field1, StringFieldTwo = field2};

            var result = instance.GetStringPropertiesAndValues();
            var parts = result.Split('\n');
            Assert.That(parts.Length, Is.EqualTo(2));

            var field1ExpectedHeader = "String Field One: ";
            var field2ExpectedHeader = "String Field Two: ";
            var field1ExpectedValue = string.IsNullOrWhiteSpace(field1) ? "[not set]" : field1;
            var field2ExpectedValue = string.IsNullOrWhiteSpace(field2) ? "[not set]" : field2;

            Assert.That(parts[0], Is.EqualTo($"{field1ExpectedHeader}{field1ExpectedValue}"));
            Assert.That(parts[1], Is.EqualTo($"{field2ExpectedHeader}{field2ExpectedValue}"));
        }

        private static readonly object[] DateCaseSource = {
            new object[]{new DateTime(2051, 1, 1), new DateTime(2016, 1, 1)},
            new object[]{null, new DateTime(2016, 1, 1)},
            new object[]{new DateTime(2051, 1, 1), null},
            new object[]{null, null}
        };

        [Test, TestCaseSource(nameof(DateCaseSource))]
        public void GetDatePropertiesAndValuesShouldReturnCorrectResult(DateTime? field1, DateTime? field2)
        {
            var instance = new TestClass { DateFieldOne = field1, DateFieldTwo = field2 };
            var result = instance.GetDatePropertiesAndValues();
            var parts = result.Split('\n');
            Assert.That(parts.Length, Is.EqualTo(2));

            var field1ExpectedHeader = "Date Field One: ";
            var field2ExpectedHeader = "Date Field Two: ";
            var field1ExpectedValue = field1?.ToString() ?? "[not set]";
            var field2ExpectedValue = field2?.ToString() ?? "[not set]";

            Assert.That(parts[0], Is.EqualTo($"{field1ExpectedHeader}{field1ExpectedValue}"));
            Assert.That(parts[1], Is.EqualTo($"{field2ExpectedHeader}{field2ExpectedValue}"));
        }

    }
}