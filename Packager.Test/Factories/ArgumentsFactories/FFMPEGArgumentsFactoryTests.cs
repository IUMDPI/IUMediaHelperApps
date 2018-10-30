using System.Collections.Generic;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories.FFMPEGArguments;
using Packager.Utilities.ProcessRunners;

namespace Packager.Test.Factories.ArgumentsFactories
{
    [TestFixture]
    public class FFMPEGArgumentsFactoryTests
    {
        private FFMPEGArgumentsFactory Factory { get; set; }

        private ArgumentBuilder NormalizingArguments { get; set; }
        private ArgumentBuilder ProdOrMezzArguments { get; set; }
        private ArgumentBuilder AccessArguments { get; set; }

        private IMediaFormat SupportedFormat1 { get; set; }
        private IMediaFormat SupportedFormat2 { get; set; }
        private IMediaFormat UnsupportedFormat { get; set; }

        private IFFMPEGArgumentsGenerator Generator1 { get; set; }
        private IFFMPEGArgumentsGenerator Generator2 { get; set; }
        
        [SetUp]
        public void BeforeEach()
        {
            NormalizingArguments = new ArgumentBuilder();
            ProdOrMezzArguments = new ArgumentBuilder();
            AccessArguments = new ArgumentBuilder();
            
            SupportedFormat1 = Substitute.For<IMediaFormat>();
            SupportedFormat2 = Substitute.For<IMediaFormat>();
            UnsupportedFormat = Substitute.For<IMediaFormat>();
            
            Generator1 = Substitute.For<IFFMPEGArgumentsGenerator>();
            Generator1.GetNormalizingArguments().Returns(NormalizingArguments);
            Generator1.GetAccessArguments().Returns(AccessArguments);
            Generator1.GetProdOrMezzArguments().Returns(ProdOrMezzArguments);

            Generator2 = Substitute.For<IFFMPEGArgumentsGenerator>();
            Generator2.GetNormalizingArguments().Returns(NormalizingArguments);
            Generator2.GetAccessArguments().Returns(AccessArguments);
            Generator2.GetProdOrMezzArguments().Returns(ProdOrMezzArguments);

            var generators = new Dictionary<IMediaFormat, IFFMPEGArgumentsGenerator>
            {
                {SupportedFormat1, Generator1},
                {SupportedFormat2, Generator2}
            };

            Factory = new FFMPEGArgumentsFactory(generators);
        }

        [Test]
        public void GetNormalizingArgumentsShouldThrowExceptionWhenFormatNotSupported()
        {
            Assert.Throws<FormatNotSupportedException>(
                () => Factory.GetNormalizingArguments(UnsupportedFormat));
        }

        [Test]
        public void GetProdOrMezzArgumentsShouldThrowExceptionWhenFormatNotSupported()
        {
            Assert.Throws<FormatNotSupportedException>(
                () => Factory.GetProdOrMezzArguments(UnsupportedFormat));
        }

        [Test]
        public void GetAccessArgumentsShouldThrowExceptionWhenFormatNotSupported()
        {
            Assert.Throws<FormatNotSupportedException>(
                () => Factory.GetAccessArguments(UnsupportedFormat));
        }

        [Test]
        public void GetNormalizingArgumentsShouldUseCorrectGeneratorForFormat()
        {
            var result = Factory.GetNormalizingArguments(SupportedFormat1);

            Generator1.Received().GetNormalizingArguments();
            Generator2.DidNotReceive().GetNormalizingArguments();

            Assert.That(result, Is.EqualTo(NormalizingArguments));
        }

        [Test]
        public void GetProdOrMessArgumentsShouldUseCorrectGeneratorForFormat()
        {
            var result = Factory.GetProdOrMezzArguments(SupportedFormat1);

            Generator1.Received().GetProdOrMezzArguments();
            Generator2.DidNotReceive().GetProdOrMezzArguments();

            Assert.That(result, Is.EqualTo(ProdOrMezzArguments));
        }

        [Test]
        public void GetAccessArgumentsShouldUseCorrectGeneratorForFormat()
        {
            var result = Factory.GetAccessArguments(SupportedFormat1);

            Generator1.Received().GetAccessArguments();
            Generator2.DidNotReceive().GetAccessArguments();

            Assert.That(result, Is.EqualTo(AccessArguments));
        }
    }
}
