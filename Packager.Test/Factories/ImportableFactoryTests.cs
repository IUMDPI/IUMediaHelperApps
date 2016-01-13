using System;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

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
                var factory = new ImportableFactory(Substitute.For<ILookupsProvider>());
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
                LookupsProvider = Substitute.For<ILookupsProvider>();
                Factory = new ImportableFactory(LookupsProvider);
            }

            private ILookupsProvider LookupsProvider { get; set; }
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

                [TestCase("first", " append" ,"first value append")]
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
                    var value = Factory.ToDateTimeValue(Parent, path);
                    Assert.That(value.HasValue, Is.True);
                    Assert.That(value.Value, Is.EqualTo(DateTime.Parse(expectedDateString)));
                }

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
            }
        }

        public class WhenResolving : ImportableFactoryTests
        {
            [SetUp]
            public virtual void BeforeEach()
            {
                LookupsProvider = Substitute.For<ILookupsProvider>();
                Factory = new ImportableFactory(LookupsProvider);
            }

            private ILookupsProvider LookupsProvider { get; set; }

            private IImportableFactory Factory { get; set; }

            private static XElement GetElement()
            {
                return new XElement("base",
                    new XElement("value") {Value = "value"},
                    new XElement("values",
                        new XElement("value1") {Value = "value 1"},
                        new XElement("value2") {Value = "value 2"}));
            }

            public class WhenResolvingTrackConfiguration : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolveTrackConfiguration(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolveTrackConfiguration(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().TrackConfiguration;
                }
            }

            public class WhenResolvingTapeThickness : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolveTapeThickness(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolveTapeThickness(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().TapeThickness;
                }
            }

            public class WhenResolvingPlaybackSpeed : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolvePlaybackSpeed(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolvePlaybackSpeed(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().PlaybackSpeed;
                }
            }

            public class WhenResolvingSoundField : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolveSoundField(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolveSoundField(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().SoundField;
                }
            }

            public class WhenResolvingDamage : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolveDamage(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolveDamage(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().Damage;
                }
            }

            public class WhenResolvingPreservationProblems : WhenResolving
            {
                [Test]
                public void IfNoValueInXmlShouldReturnEmptyString()
                {
                    var result = Factory.ResolvePreservationProblems(GetElement(), "invalid");
                    Assert.That(result, Is.EqualTo(string.Empty));
                }

                [Test]
                public void ItShouldCallLookupsProviderCorrectly()
                {
                    Factory.ResolvePreservationProblems(GetElement(), "values");
                    // need to assign to variable to make compiler happy
                    var test = LookupsProvider.Received().PreservationProblem;
                }
            }
        }
    }
}