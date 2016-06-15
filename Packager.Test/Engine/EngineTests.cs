using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Engine;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.UserInterface;
using Packager.Utilities.Configuration;
using Packager.Utilities.FileSystem;
using Packager.Validators;

namespace Packager.Test.Engine
{
    [TestFixture]
    public class EngineTests
    {
        [SetUp]
        public virtual void BeforeEach()
        {
            SuccessFolderCleaner = Substitute.For<ISuccessFolderCleaner>();
            ViewModel = Substitute.For<IViewModel>();
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.ProjectCode.Returns(ProjectCode);

            Validators = Substitute.For<IValidatorCollection>();

            MockWavProcessor = Substitute.For<IProcessor>();
            MockMpegProcessor = Substitute.For<IProcessor>();

            MockWavProcessor.ProcessObject(null, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Task.FromResult(ValidationResult.Success));
            MockMpegProcessor.ProcessObject(null, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Task.FromResult(ValidationResult.Success));

            Observer = Substitute.For<IObserverCollection>();

            Grouping1PresFileName = GetPresFileNameForBarCode(BarCode1, MockWavProcessorExtension);
            Grouping1ProdFileName = GetProdFileNameForBarCode(BarCode1, MockWavProcessorExtension);
            Grouping2PresFileName = GetPresFileNameForBarCode(BarCode2, MockMkvProcessorExtension);

            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(
                new List<string>
                {
                    Grouping1PresFileName,
                    Grouping1ProdFileName,
                    Grouping2PresFileName
                });
            
            Validators.Validate(ProgramSettings).Returns(new ValidationResults());

            var processors = new Dictionary<string, IProcessor>
            {
                {MockWavProcessorExtension, MockWavProcessor},
                {MockMkvProcessorExtension, MockMpegProcessor}
            };

            Engine = new StandardEngine(processors, ViewModel, ProgramSettings, DirectoryProvider, Validators,
                SuccessFolderCleaner, Substitute.For<IConfigurationLogger>(), Observer);
        }

        private const string MockWavProcessorExtension = ".wav";
        private const string MockMkvProcessorExtension = ".mkv";
        private const string ProjectCode = "MDPI";
        private const string BarCode1 = "4890764553278906";
        private const string BarCode2 = "7890764553278907";
        private StandardEngine Engine { get; set; }
        private IObserverCollection Observer { get; set; }
        private IProcessor MockWavProcessor { get; set; }
        private IProcessor MockMpegProcessor { get; set; }
        private IProgramSettings ProgramSettings { get; set; }
        private IDirectoryProvider DirectoryProvider { get; set; }
        private IViewModel ViewModel { get; set; }
        private string Grouping1PresFileName { get; set; }
        private string Grouping1ProdFileName { get; set; }
        private string Grouping2PresFileName { get; set; }
        

        private IValidatorCollection Validators { get; set; }
        private ISuccessFolderCleaner SuccessFolderCleaner { get; set; }

        private static string GetPresFileNameForBarCode(string barcode, string extension)
        {
            return $"{ProjectCode}_{barcode}_01_pres{extension}";
        }

        private static string GetProdFileNameForBarCode(string barcode, string extension)
        {
            return $"{ProjectCode}_{barcode}_01_prod{extension}";
        }

        public class WhenEngineRunsWithoutIssues : EngineTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                await Engine.Start(CancellationToken.None);
            }

            [Test]
            public void ItShouldCallProcessorForEachKnownExtension()
            {
                MockWavProcessor.Received()
                    .ProcessObject(Arg.Is<IGrouping<string, AbstractFile>>(g => g.Key.Equals(BarCode1)), Arg.Any<CancellationToken>());
                MockMpegProcessor.Received()
                    .ProcessObject(Arg.Is<IGrouping<string, AbstractFile>>(g => g.Key.Equals(BarCode2)), Arg.Any<CancellationToken>());
            }

            [Test]
            public void ItShouldLogObjectCountCorrectly()
            {
                Observer.Received().Log("Found {0} to process", Arg.Any<string>());
            }

            [Test]
            public void ItShouldSendCorrectGroupingsToProcessors()
            {
                MockWavProcessor.Received().ProcessObject(
                    Arg.Is<IGrouping<string, AbstractFile>>(g => g.Count() == 2), 
                    Arg.Any<CancellationToken>());
                MockWavProcessor.Received().ProcessObject(
                    Arg.Is<IGrouping<string, AbstractFile>>(g => g.SingleOrDefault(m => m.Filename.Equals(Grouping1PresFileName)) !=null), 
                    Arg.Any<CancellationToken>());
                MockWavProcessor.Received().ProcessObject(
                     Arg.Is<IGrouping<string, AbstractFile>>(g =>g.AsEnumerable().SingleOrDefault(m => m.Filename.Equals(Grouping1ProdFileName)) !=null),
                     Arg.Any<CancellationToken>());

                MockMpegProcessor.Received().ProcessObject(
                    Arg.Is<IGrouping<string, AbstractFile>>(g => g.Count() == 1),
                    Arg.Any<CancellationToken>());
                MockMpegProcessor.Received().ProcessObject(
                    Arg.Is<IGrouping<string, AbstractFile>>(g => g.AsEnumerable().SingleOrDefault(m => m.Filename.Equals(Grouping2PresFileName)) !=null),
                    Arg.Any<CancellationToken>());
            }

            [Test]
            public void ItShouldValidateDependencyProvider()
            {
                //Validators.Received().Validate(DependencyProvider);
            }

            [Test]
            public void ItShouldWriteGoodbyeMessage()
            {
                Observer.Received().Log(Arg.Is("Completed {0}"), Arg.Any<DateTime>());
            }

            [Test]
            public void ItShouldWriteHelloMessage()
            {
                Observer.Received().Log(Arg.Is("Starting {0} (version {1})"), Arg.Any<DateTime>(), Arg.Any<Version>());
            }
        }

        public class WhenProcessorEncountersAnIssue : EngineTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                MockWavProcessor.ProcessObject(null, Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Task.FromResult(new ValidationResult("issue")));

                await Engine.Start(CancellationToken.None);
            }
        }

        public class WhenEngineEncountersAnIssue : EngineTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                Exception = new DirectoryNotFoundException("invalid path");

                DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(r => { throw Exception; });

                await Engine.Start(CancellationToken.None);
            }

            private Exception Exception { get; set; }

            [Test]
            public void ItShouldWriteErrorMessage()
            {
                Observer.Received().LogEngineIssue(Exception);
            }
        }
    }
}