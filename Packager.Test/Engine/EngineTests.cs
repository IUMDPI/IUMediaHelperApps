using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Engine;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;

namespace Packager.Test.Engine
{
    [TestFixture]
    public class EngineTests
    {
        protected const string ProjectCode = "mdpi";
        protected const string BarCode1 = "4890764553278906";
        protected const string BarCode2 = "7890764553278907";

        protected string GetFileNameForBarCode(string projectCode, string barcode, string extension)
        {
            return string.Format("{0}_{1}_01_pres{2}", projectCode, barcode, extension);
        }

        private static StandardEngine GetEngine(
            IProgramSettings settings = null,
            Dictionary<string, IProcessor> processors = null,
            IDependencyProvider utilityProvider = null,
            List<IObserver> observers = null)
        {
            if (settings == null)
            {
                settings = Substitute.For<IProgramSettings>();
                settings.ProjectCode.ReturnsForAnyArgs(ProjectCode);
            }

            if (processors == null)
            {
                var mockProcessor = Substitute.For<IProcessor>();
                processors = new Dictionary<string, IProcessor> {{".wav", mockProcessor}};
            }

            if (utilityProvider == null)
            {
                utilityProvider = Substitute.For<IDependencyProvider>();
            }

            if (observers == null)
            {
                var mockObserver = Substitute.For<IObserver>();
                observers = new List<IObserver> {mockObserver};
            }

            return new StandardEngine(settings, processors, utilityProvider, observers);
        }

        public class WhenEngineRunsWithoutIssues : EngineTests
        {
            [Test]
            public void ItShouldVerifyProgramSettings()
            {
                var settings = Substitute.For<IProgramSettings>();

                var engine = GetEngine(settings);
                engine.Start();

                settings.Received().Verify();
            }

            [Test]
            public void ItShouldWriteHelloMessage()
            {
                var mockObserver = Substitute.For<IObserver>();
                var engine = GetEngine(observers: new List<IObserver> {mockObserver});
                engine.Start();

                mockObserver.Received().Log(Arg.Is("Starting {0}"), Arg.Any<DateTime>());
            }

            [Test]
            public void ItShouldWriteGoodbyeMessage()
            {
                var mockObserver = Substitute.For<IObserver>();
                var engine = GetEngine(observers: new List<IObserver> {mockObserver});
                engine.Start();

                mockObserver.Received().Log(Arg.Is("Completed {0}"), Arg.Any<DateTime>());
            }

            [Test]
            public void ItShouldCallProcessorForEachKnownExtension()
            {
                var mockWavProcessor = Substitute.For<IProcessor>();
                var mockMpegProcessor = Substitute.For<IProcessor>();

                var processors = new Dictionary<string, IProcessor>
                {
                    {".wav", mockWavProcessor},
                    {".mpeg", mockMpegProcessor}
                };

                var directoryProvider = Substitute.For<IDirectoryProvider>();
                directoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(
                    new List<string>
                    {
                        GetFileNameForBarCode(ProjectCode, BarCode1, ".wav"),
                        GetFileNameForBarCode(ProjectCode, BarCode2, ".mpeg")
                    });

                var utilityProvider = Substitute.For<IDependencyProvider>();
                utilityProvider.DirectoryProvider.ReturnsForAnyArgs(directoryProvider);

                var engine = GetEngine(processors: processors, utilityProvider: utilityProvider);
                engine.Start();

                mockWavProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Key.Equals(BarCode1)));
                mockMpegProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Key.Equals(BarCode2)));
            }
        }

        public class WhenEngineEncountersAnIssue : EngineTests
        {
            [Test]
            public void ItShouldWriteErrorMessage()
            {
                var mockObserver = Substitute.For<IObserver>();

                var exception = new DirectoryNotFoundException("invalid path");
                var directoryProvider = Substitute.For<IDirectoryProvider>();

                directoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(r => { throw exception; });

                var utilityProvider = Substitute.For<IDependencyProvider>();
                utilityProvider.DirectoryProvider.Returns(directoryProvider);

                var engine = GetEngine(observers: new List<IObserver> {mockObserver}, utilityProvider: utilityProvider);
                engine.Start();

                mockObserver.Received().Log(Arg.Is("Fatal Exception Occurred: {0}"), Arg.Is(exception));
            }
        }
    }
}