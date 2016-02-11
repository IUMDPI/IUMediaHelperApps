using System;
using System.Xml.Linq;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class ImportableFactoryTests
    {
        [TestFixture]
        public class WhenImportingMetadataFromXElement : ImportableFactoryTests
        {
            private static XElement GetElement()
            {
                return new XElement("pod",
                    new XElement("success") {Value = "true"},
                    new XElement("data",
                        new XElement("object",
                            new XElement("details",
                                new XElement("format") {Value = "open reel audio tape"}))));
            }

            // here, we're not doing exhaustive tests, just making sure that
            // the factory method returns expected given bare minimum needed
            // to make it succeed. 
            [Test]
            public void ItShouldReturnExpectedInstance()
            {
                var factory = new ImportableFactory();
                var result = factory.ToImportable<VideoPodMetadata>(GetElement());
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.EqualTo(true));
            }
        }


        public class WhenImportingPrimitives : ImportableFactoryTests
        {
            [SetUp]
            public virtual void BeforeEach()
            {
                Factory = new ImportableFactory();
            }

            private IImportableFactory Factory { get; set; }

            private XElement Parent { get; set; }

            public class WhenImportingStringValues : WhenImportingPrimitives
            {
                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    Parent = new XElement("root",
                        new XElement("first") {Value = "first value"},
                        new XElement("second") {Value = "second value"},
                        new XElement("empty") {Value = ""});
                }

                [TestCase("first", " append", "first value append")]
                [TestCase("second", "", "second value")]
                [TestCase("second", " ", "second value")]
                [TestCase("empty", " append", "")]
                public void IfPathIsValidShouldReturnCorrectValue(string path, string append, string expected)
                {
                    var value = Factory.ToStringValue(Parent, path, append);
                    Assert.That(value, Is.EqualTo(expected));
                }

                [Test]
                public void IfPathIsNotValidShouldReturnEmptyString()
                {
                    var value = Factory.ToStringValue(Parent, "first/invalid");
                    Assert.That(value, Is.EqualTo(string.Empty));
                }
            }


            public class WhenImportingBooleanValues : WhenImportingPrimitives
            {
                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

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
                    var value = Factory.ToBooleanValue(Parent, path);
                    Assert.That(value, Is.EqualTo(expected));
                }

                [TestCase("empty")]
                [TestCase("invalid")]
                public void IfValueIsEmptyOrInvalidShouldReturnFalse(string path)
                {
                    var value = Factory.ToBooleanValue(Parent, path);
                    Assert.That(value, Is.False);
                }

                [Test]
                public void IfPathIsNotValidShouldReturnFalse()
                {
                    var value = Factory.ToBooleanValue(Parent, "first/invalid");
                    Assert.That(value, Is.False);
                }
            }

            public class WhenImportingDateTimeValues : WhenImportingPrimitives
            {
                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    Parent = new XElement("root",
                        new XElement("first") {Value = FirstDate},
                        new XElement("second") {Value = SecondDate},
                        new XElement("third") {Value = ThirdDate},
                        new XElement("fourth") {Value = FourthDate},
                        new XElement("invalid1") {Value = InvalidDate},
                        new XElement("invalid2") {Value = "invalid"},
                        new XElement("empty") {Value = ""});
                }

                private const string FirstDate = "2015-11-23T00:00:00Z";
                private const string SecondDate = "2015-07-29T10:28:21Z";
                private const string ThirdDate = "2015-06-30T10:28:21Z";
                private const string FourthDate = "2015-05-29T10:28:21Z";
                private const string InvalidDate = "2015-06-33T10:28:21Z";


                private static readonly object[] DateCases =
                {
                    new object[] {"first", new DateTime(2015, 11, 23)},
                    new object[] {"second", new DateTime(2015, 7, 29, 10, 28, 21)},
                    new object[] {"third", new DateTime(2015, 6, 30, 10, 28, 21)},
                    new object[] {"fourth", new DateTime(2015, 5, 29, 10, 28, 21)}
                };

                [TestCase("empty")]
                [TestCase("invalid1")]
                [TestCase("invalid2")]
                public void IfValueIsEmptyOrInvalidShouldNotHaveValue(string path)
                {
                    var value = Factory.ToDateTimeValue(Parent, path);
                    Assert.That(value.HasValue, Is.False);
                }

                [Test]
                public void IfPathIsNotValidShouldShouldNotHaveValue()
                {
                    var value = Factory.ToDateTimeValue(Parent, "first/invalid");
                    Assert.That(value.HasValue, Is.False);
                }
                
                [Test, TestCaseSource(nameof(DateCases))]
                public void IfPathIsValidShouldReturnCorrectValue(string path, DateTime expected)
                {
                    var value = Factory.ToDateTimeValue(Parent, path);
                    Assert.That(value.HasValue, Is.True);
                    Assert.That(value.Value, Is.EqualTo(expected));
                }
            }
        }
    }
}