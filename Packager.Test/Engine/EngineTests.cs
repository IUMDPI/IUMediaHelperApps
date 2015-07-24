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

        private static string GetFileNameForBarCode(string projectCode, string barcode, string extension)
        {
            return string.Format("{0}_{1}_01_pres{2}", projectCode, barcode, extension);
        }

        private StandardEngine Engine { get; set; }
        private IObserver Observer { get; set; }
        private IDependencyProvider DependencyProvider { get; set; }
        private IProcessor MockWavProcessor { get; set; }
        private IProcessor MockMpegProcessor { get; set; }
        private IProgramSettings ProgramSettings { get; set; }
        private IDirectoryProvider DirectoryProvider { get; set; }

        [SetUp]
        public virtual void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.ProjectCode.Returns(ProjectCode);
            
            MockWavProcessor = Substitute.For<IProcessor>();
            MockMpegProcessor = Substitute.For<IProcessor>();

            MockWavProcessor.ProcessFile(null).ReturnsForAnyArgs(Task.FromResult(true));
            MockMpegProcessor.ProcessFile(null).ReturnsForAnyArgs(Task.FromResult(true));

            Observer = Substitute.For<IObserver>();

            DirectoryProvider = Substitute.For<IDirectoryProvider>();

            DependencyProvider = MockDependencyProvider.Get(observers: new List<IObserver> {Observer}, programSettings: ProgramSettings, directoryProvider: DirectoryProvider);
            Engine = new StandardEngine(
                new Dictionary<string, IProcessor>
                {
                    {MockWavProcessorExtension, MockWavProcessor}, 
                    {MockMpegProcessorExtension, MockMpegProcessor}
                }, DependencyProvider);
        }

        public class WhenEngineRunsWithoutIssues : EngineTests
        {
            public async override void BeforeEach()
            {
                base.BeforeEach();

                DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(
                   new List<string>
                    {
                        GetFileNameForBarCode(ProjectCode, BarCode1, ".wav"),
                        GetFileNameForBarCode(ProjectCode, BarCode2, ".mpeg")
                    });


                await Engine.Start();
            }

            [Test]
            public void ItShouldVerifyProgramSettings()
            {
                ProgramSettings.Received().Verify();
            }

            [Test]
            public void ItShouldWriteHelloMessage()
            {
                Observer.Received().Log(Arg.Is("Starting {0}"), Arg.Any<DateTime>());
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
        }

        public class WhenEngineEncountersAnIssue : EngineTests
        {
            private Exception Exception { get; set; }

            public async override void BeforeEach()
            {
                base.BeforeEach();

                Exception = new DirectoryNotFoundException("invalid path");

                DirectoryProvider.EnumerateFiles(null).ReturnsForAnyArgs(r => { throw Exception; });

                await Engine.Start();
            }

            [Test]
            public void ItShouldWriteErrorMessage()
            {
                Observer.Received().LogError(Exception);
            }
        }
    }
}