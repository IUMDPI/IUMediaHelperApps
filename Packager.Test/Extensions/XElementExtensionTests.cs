/*using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class XElementExtensionTests
    {
        private XElement Parent { get; set; }
     
        public class WhenConvertingToStringValues : XElementExtensionTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Parent = new XElement("root",
                    new XElement("first") {Value = "first value"},
                    new XElement("second") {Value = "second value"},
                    new XElement("empty") {Value = ""});
            }

            [TestCase("first", "first value")]
            [TestCase("second", "second value")]
            [TestCase("empty", "")]
            public void IfPathIsValidShouldReturnCorrectValue(string path, string expected)
            {
                var value = Parent.ToStringValue(path);
                Assert.That(value, Is.EqualTo(expected));
            }

            [Test]
            public void IfPathIsNotValidShouldReturnEmptyString()
            {
                var value = Parent.ToStringValue("first/invalid");
                Assert.That(value, Is.EqualTo(string.Empty));
            }
        }

        public class WhenConvertingToBooleanValues : XElementExtensionTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Parent = new XElement("root",
                    new XElement("first") {Value = "true"},
                    new XElement("second") {Value = "True"},
                    new XElement("third") {Value = "false"},
                    new XElement("fourth") {Value = "False"},
                    new XElement("invalid") {Value = "invalid"},
                    new XElement("empty") {Value = ""});
            }

            [TestCase("first", true)]
            [TestCase("second", true)]
            [TestCase("third", false)]
            [TestCase("fourth", false)]
            public void IfPathIsValidShouldReturnCorrectValue(string path, bool expected)
            {
                var value = Parent.ToBooleanValue(path);
                Assert.That(value, Is.EqualTo(expected));
            }

            [TestCase("empty")]
            [TestCase("invalid")]
            public void IfValueIsEmptyOrInvalidShouldReturnFalse(string path)
            {
                var value = Parent.ToBooleanValue(path);
                Assert.That(value, Is.False);
            }


            [Test]
            public void IfPathIsNotValidShouldReturnFalse()
            {
                var value = Parent.ToBooleanValue("first/invalid");
                Assert.That(value, Is.False);
            }
        }

        public class WhenConvertingToDateTimeValues : XElementExtensionTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Parent = new XElement("root",
                    new XElement("first") {Value = FirstDate},
                    new XElement("second") {Value = SecondDate},
                    new XElement("third") {Value = ThirdDate},
                    new XElement("fourth") {Value = FourthDate},
                    new XElement("invalid1") {Value = InvalidDate},
                    new XElement("invalid2") {Value = "invalid"},
                    new XElement("empty") {Value = ""});
            }

            private const string FirstDate = "2015-06-29T10:28:21-04:00";
            private const string SecondDate = "2015-07-29T10:28:21-04:00";
            private const string ThirdDate = "2015-06-30T10:28:21-04:00";
            private const string FourthDate = "2015-05-29T10:28:21-04:00";
            private const string InvalidDate = "2015-06-33T10:28:21-04:00";

            [TestCase("first", FirstDate)]
            [TestCase("second", SecondDate)]
            [TestCase("third", ThirdDate)]
            [TestCase("fourth", FourthDate)]
            public void IfPathIsValidShouldReturnCorrectValue(string path, string expectedDateString)
            {
                var value = Parent.ToDateTimeValue(path);
                Assert.That(value.HasValue, Is.True);
                Assert.That(value.Value, Is.EqualTo(DateTime.Parse(expectedDateString)));
            }

            [TestCase("empty")]
            [TestCase("invalid1")]
            [TestCase("invalid2")]
            public void IfValueIsEmptyOrInvalidShouldNotHaveValue(string path)
            {
                var value = Parent.ToDateTimeValue(path);
                Assert.That(value.HasValue, Is.False);
            }


            [Test]
            public void IfPathIsNotValidShouldShouldNotHaveValue()
            {
                var value = Parent.ToDateTimeValue("first/invalid");
                Assert.That(value.HasValue, Is.False);
            }
        }


        public class WhenResolvingToDictionaryValues : XElementExtensionTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Parent = new XElement("root",
                    new XElement("first", new XElement("value1") {Value="true"}),
                    new XElement("second", new XElement("value1") { Value = "true" }, new XElement("value2") { Value = "true" }),
                    new XElement("third", new XElement("value1") { Value = "true" }, new XElement("value2") { Value = "false" }),
                    new XElement("fourth") { Value = "Value 1" },
                    new XElement("fifth") { Value = "value 1" }, // note case difference
                    new XElement("invalid1", new XElement("invalid") { Value = "true" }),
                    new XElement("invalid2") {Value = "invalid"},
                    new XElement("empty") {Value = ""});

                LookupsDictionary = new Dictionary<string, string>
                {
                    {"value1", "Value 1" },
                    {"value2", "Value 2" }
                };
            }

            private Dictionary<string, string> LookupsDictionary { get; set; }

            [TestCase("first", "Value 1")]
            [TestCase("second", "Value 1,Value 2")]
            [TestCase("third", "Value 1")]
            [TestCase("fourth", "Value 1")]
            [TestCase("fifth", "Value 1")]
            public void IfPathIsValidShouldReturnCorrectValue(string path, string expected)
            {
                var value = Parent.ToResolvedDelimitedString(path, LookupsDictionary);
                Assert.That(value, Is.EqualTo(expected));
            }

            [TestCase("empty")]
            [TestCase("invalid1")]
            [TestCase("invalid2")]
            public void IfValueIsEmptyOrInvalidShouldReturnEmptyString(string path)
            {
                var value = Parent.ToResolvedDelimitedString(path, LookupsDictionary);
                Assert.That(value, Is.EqualTo(string.Empty));
            }


            [Test]
            public void IfPathIsNotValidShouldShouldReturnEmptyString()
            {
                var value = Parent.ToResolvedDelimitedString("first/invalid", LookupsDictionary);
                Assert.That(value, Is.EqualTo(string.Empty));
            }
        }
    }
}*/