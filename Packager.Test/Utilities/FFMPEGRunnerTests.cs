using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;
using Packager.Validators.Attributes;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class FFMPEGRunnerTests
    {
        private const string FFMPEGPath = "ffmpeg.exe";
        private const string Arguments = "arguments";
        private const string BaseProcessingDirectory = "base processing directory";
        private const string OutputFolder = "output folder";
        private const string MasterFileName = "MDPI_123456789_01_pres.wav";
        private const string DerivativeFileName = "MDPI_123456789_01_access.wav";
        private ObjectFileModel MasterFileModel { get; set; }
        private ObjectFileModel DerivativeFileModel { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IObserverCollection Observers { get; set; }
        private IFileProvider FileProvider { get; set; }
        private FFMPEGRunner Runner { get; set; }
        private ObjectFileModel Result { get; set; }

        protected virtual void DoCustomSetup()
        {
        }

        [SetUp]
        public async void BeforeEach()
        {
            MasterFileModel = new ObjectFileModel(MasterFileName);
            DerivativeFileModel = new ObjectFileModel(DerivativeFileName);
            ProcessRunner = Substitute.For<IProcessRunner>();
            Observers = Substitute.For<IObserverCollection>();
            FileProvider = Substitute.For<IFileProvider>();

            Runner = new FFMPEGRunner(FFMPEGPath, BaseProcessingDirectory, ProcessRunner, Observers, FileProvider);

            DoCustomSetup();

            Result = await Runner.CreateDerivative(MasterFileModel, DerivativeFileModel, Arguments, OutputFolder);
        }

        [Test]
        public void FFMPEGPathPropertyShouldHaveValidateAttribute()
        {
            var info = typeof (FFMPEGRunner).GetProperty("FFMPEGPath");
            Assert.That(info.GetCustomAttribute<ValidateFileAttribute>(), Is.Not.Null);
        }

        [Test]
        public void ItShouldCallBeginSectionCorrectly()
        {
            Observers.Received().BeginSection("Generating {0}: {1}", DerivativeFileModel.FullFileUse, DerivativeFileModel.ToFileName());
        }

        public class WhenThingsGoWell : FFMPEGRunnerTests
        {
            [Test]
            public void ItShouldCallEndSectionCorrectly()
            {
                Observers.Received().EndSection(Arg.Any<Guid>(), string.Format("{0} generated successfully: {1}", DerivativeFileModel.FullFileUse, DerivativeFileModel.ToFileName()));
            }

            [Test]
            public void ItShouldReturnCorrectResult()
            {
                Assert.That(Result, Is.EqualTo(DerivativeFileModel));
            }

            public class WhenDerivativeAlreadyExists : WhenThingsGoWell
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    FileProvider.FileExists(null).ReturnsForAnyArgs(true);
                }

                [Test]
                public void ItShouldLogThatFileAlreadyExists()
                {
                    Observers.Received().Log("{0} already exists. Will not generate derivative", DerivativeFileModel.FullFileUse);
                }

                [Test]
                public void ItShouldNotCallProcessRunner()
                {
                    ProcessRunner.DidNotReceive().Run(Arg.Any<ProcessStartInfo>());
                }
            }

            public class WhenDerivativeDoesNotExist : WhenThingsGoWell
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    FileProvider.FileExists(null).ReturnsForAnyArgs(false);
                }

                [Test]
                public void ItShouldCallProcessRunnerCorrectly()
                {
                    var expectedArgs = string.Format("-i {0} {1} {2}",
                        Path.Combine(OutputFolder, MasterFileName),
                        Arguments,
                        Path.Combine(OutputFolder, DerivativeFileName));

                    ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(
                        i => i.FileName.Equals(FFMPEGPath) &&
                             i.Arguments.Equals(expectedArgs) &&
                             i.RedirectStandardError &&
                             i.RedirectStandardOutput &&
                             i.CreateNoWindow &&
                             i.UseShellExecute == false));
                }
            }
        }

        }
}