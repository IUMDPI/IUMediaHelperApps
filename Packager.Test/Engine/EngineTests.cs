using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Engine;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Test.Mocks;
using Packager.Validators;

namespace Packager.Test.Engine
{
    [TestFixture]
    public class EngineTests
    {
        private const string MockWavProcessorExtension = ".wav";
        private const string MockMpegProcessorExtension = ".mpeg";
        private const string ProjectCode = "mdpi";
        private const string BarCode1 = "4890764553278906";
        private const string BarCode2 = "7890764553278907";
        private StandardEngine Engine { get; set; }
        private IObserverCollection Observer { get; set; }
        private IDependencyProvider DependencyProvider { get; set; }
        private IProcessor MockWavProcessor { get; set; }
        private IProcessor MockMpegProcessor { get; set; }
        private IProgramSettings ProgramSettings { get; set; }
        private IDirectoryProvider DirectoryProvider { get; set; }
        private string Grouping1PresFileName { get; set; }
        private string Grouping1ProdFileName { get; set; }
        private string Grouping2PresFileName { get; set; }

        private IValidatorCollection Validators { get; set; }


        private static string GetPresFileNameForBarCode(string barcode, string extension)
        {
            return string.Format("{0}_{1}_01_pres{2}", ProjectCode, barcode, extension);
        }

        private static string GetProdFileNameForBarCode(string barcode, string extension)
        {
            return string.Format("{0}_{1}_01_prod{2}", ProjectCode, barcode, extension);
        }

        [SetUp]
        public virtual void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.ProjectCode.Returns(ProjectCode);

            Validators = Substitute.For<IValidatorCollection>();
            Validators.Validate(ProgramSettings).Returns(new ValidationResults());

            MockWavProcessor = Substitute.For<IProcessor>();
            MockMpegProcessor = Substitute.For<IProcessor>();

            MockWavProcessor.ProcessFile(null).ReturnsForAnyArgs(Task.FromResult(true));
            MockMpegProcessor.ProcessFile(null).ReturnsForAnyArgs(Task.FromResult(true));

            Observer = Substitute.For<IObserverCollection>();

            Grouping1PresFileName = GetPresFileNameForBarCode(BarCode1, MockWavProcessorExtension);
            Grouping1ProdFileName = GetProdFileNameForBarCode(BarCode1, MockWavProcessorExtension);
            Grouping2PresFileName = GetPresFileNameForBarCode(BarCode2, MockMpegProcessorExtension);

            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(
                new List<string>
                {
                    Grouping1PresFileName,
                    Grouping1ProdFileName,
                    Grouping2PresFileName
                });

            DependencyProvider = MockDependencyProvider.Get(observers: Observer, programSettings: ProgramSettings, directoryProvider: DirectoryProvider, validators:Validators);
            Engine = new StandardEngine(
                new Dictionary<string, IProcessor>
                {
                    {MockWavProcessorExtension, MockWavProcessor},
                    {MockMpegProcessorExtension, MockMpegProcessor}
                }, DependencyProvider);
        }

        public class WhenEngineRunsWithoutIssues : EngineTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                await Engine.Start();
            }

            [Test]
            public void ItShouldValidateProgramSettings()
            {
                Validators.Received().Validate(ProgramSettings);
            }

            [Test]
            public void ItShouldWriteHelloMessage()
            {
                Observer.Received().Log(Arg.Is("Starting {0} (version {1})"), Arg.Any<DateTime>(), Arg.Any<Version>());
            }

            [Test]
            public void ItShouldReportResultsCorrectly()
            {
                Observer.Received().Log("Successfully processed {0} objects.", 2);
            }

            [Test]
            public void ItShouldLogObjectCountCorrectly()
            {
                Observer.Received().Log("Found {0} objects to process", 2);
            }

            [Test]
            public void ItShouldWriteGoodbyeMessage()
            {
                Observer.Received().Log(Arg.Is("Completed {0}"), Arg.Any<DateTime>());
            }

            [Test]
            public void ItShouldCallProcessorForEachKnownExtension()
            {
                MockWavProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Key.Equals(BarCode1)));
                MockMpegProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Key.Equals(BarCode2)));
            }

            [Test]
            public void ItShouldSendCorrectGroupingsToProcessors()
            {
                MockWavProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Count() == 2));
                MockWavProcessor.Received()
                    .ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.AsEnumerable().SingleOrDefault(m => m.OriginalFileName.Equals(Grouping1PresFileName)) != null));
                MockWavProcessor.Received()
                    .ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.AsEnumerable().SingleOrDefault(m => m.OriginalFileName.Equals(Grouping1ProdFileName)) != null));

                MockMpegProcessor.Received().ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.Count() == 1));
                MockMpegProcessor.Received()
                    .ProcessFile(Arg.Is<IGrouping<string, AbstractFileModel>>(g => g.AsEnumerable().SingleOrDefault(m => m.OriginalFileName.Equals(Grouping2PresFileName)) != null));
            }
        }

        public class WhenProcessorEncountersAnIssue : EngineTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                MockWavProcessor.ProcessFile(null).ReturnsForAnyArgs(Task.FromResult(false));

                await Engine.Start();
            }

            [Test]
            public void ShouldReportFailureCorrectly()
            {
                Observer.Received().Log("Could not process {0}", BarCode1);
            }
        }

        public class WhenEngineEncountersAnIssue : EngineTests
        {
            private Exception Exception { get; set; }

            public override async void BeforeEach()
            {
                base.BeforeEach();

                Exception = new DirectoryNotFoundException("invalid path");

                DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(r => { throw Exception; });

                await Engine.Start();
            }

            [Test]
            public void ItShouldWriteErrorMessage()
            {
                Observer.Received().LogEngineIssue(Exception);
            }
        }
    }
}