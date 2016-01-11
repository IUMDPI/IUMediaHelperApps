using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public class WhenImportingMetadataFromXElement: ImportableFactoryTests
        {
            private static XElement GetElement()
            {
                return new XElement("pod",
                    new XElement("success") { Value = "true" },
                    new XElement("data",
                        new XElement("object",
                            new XElement("details",
                                new XElement("format") { Value = "open reel audio tape" }))));
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

        public class WhenResolving : ImportableFactoryTests
        {
            private ILookupsProvider LookupsProvider { get; set; }

            private IImportableFactory Factory { get; set; }

            private static XElement GetElement()
            {
                return new XElement("base", 
                    new XElement("values", 
                        new XElement("value1") {Value = "value 1"},
                        new XElement("value2") { Value = "value 2" }));
            }

            [SetUp]
            public void BeforeEach()
            {
                LookupsProvider = Substitute.For<ILookupsProvider>();
                Factory = new ImportableFactory(LookupsProvider);
            }

            public class WhenResolvingTrackConfiguration : WhenResolving
            {
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
