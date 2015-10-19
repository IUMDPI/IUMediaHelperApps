using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
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
        private const string BaseProcessingDirectory = "base";
        private const string MasterFileName = "MDPI_123456789_01_pres.wav";
        private const string DerivativeFileName = "MDPI_123456789_01_access.wav";
        private const string StandardErrorOutput = "Standard error output";
        private ObjectFileModel MasterFileModel { get; set; }
        private ObjectFileModel DerivativeFileModel { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IObserverCollection Observers { get; set; }
        private IFileProvider FileProvider { get; set; }
        private FFMPEGRunner Runner { get; set; }
        private ObjectFileModel Result { get; set; }
        private IProcessResult ProcessRunnerResult { get; set; }

        private IHasher Hasher { get; set; }


        protected virtual void DoCustomSetup()
        {
        }

        [SetUp]
        public virtual void BeforeEach()
        {
            ProcessRunnerResult = Substitute.For<IProcessResult>();
            ProcessRunnerResult.ExitCode.Returns(0);
            ProcessRunnerResult.StandardError.Returns(StandardErrorOutput);

            MasterFileModel = new ObjectFileModel(MasterFileName);
            DerivativeFileModel = new ObjectFileModel(DerivativeFileName);
            ProcessRunner = Substitute.For<IProcessRunner>();
           
            Observers = Substitute.For<IObserverCollection>();
            FileProvider = Substitute.For<IFileProvider>();

            Hasher = Substitute.For<IHasher>();

            Runner = new FFMPEGRunner(FFMPEGPath, BaseProcessingDirectory, ProcessRunner, Observers, FileProvider, Hasher);

            DoCustomSetup();
        }

        [Test]
        public void FFMPEGPathPropertyShouldHaveValidateAttribute()
        {
            var info = typeof(FFMPEGRunner).GetProperty("FFMPEGPath");
            Assert.That(info.GetCustomAttribute<ValidateFileAttribute>(), Is.Not.Null);
        }


        public class WhenThingsGoWell : FFMPEGRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                {
                    ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                    return Task.FromResult(ProcessRunnerResult);
                });

                Result = await Runner.CreateDerivative(MasterFileModel, DerivativeFileModel, Arguments);
            }

            [Test]
            public void ItShouldCallBeginSectionCorrectly()
            {
                Observers.Received().BeginSection("Generating {0}: {1}", DerivativeFileModel.FullFileUse, DerivativeFileModel.ToFileName());
            }

            [Test]
            public void ItShouldCallEndSectionCorrectly()
            {
                Observers.Received().EndSection(Arg.Any<string>(), $"{DerivativeFileModel.FullFileUse} generated successfully: {DerivativeFileModel.ToFileName()}");
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
                public void ItShouldLogProcessResultOutput()
                {
                    Observers.Received().Log(StandardErrorOutput);
                }

                [Test]
                public void ItShouldCallProcessRunnerCorrectly()
                {
                    var expectedArgs = string.Format("-i {0} {1} {2}",
                        Path.Combine(BaseProcessingDirectory, MasterFileModel.GetFolderName(), MasterFileName),
                        Arguments,
                        Path.Combine(BaseProcessingDirectory, MasterFileModel.GetFolderName(), DerivativeFileName));

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

        public class WhenThingsGoWrong : FFMPEGRunnerTests
        {
            private LoggedException FinalException { get; set; }

            public class WhenProcessRunnerReturnsFailResult : WhenThingsGoWrong
            {
                public override void BeforeEach()
                {
                    base.BeforeEach();
                    ProcessRunnerResult.ExitCode.Returns(-1);
                    ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                    {
                        ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                        return Task.FromResult(ProcessRunnerResult);
                    });

                    FinalException = Assert.Throws<LoggedException>(async () => await Runner.CreateDerivative(MasterFileModel, DerivativeFileModel, Arguments));
                }
                
                [Test]
                public void FinalExceptionShouldBeCorrect()
                {
                    var innerException = FinalException.InnerException as GenerateDerivativeException;
                    Assert.That(innerException, Is.Not.Null);
                    Assert.That(innerException.Message, Is.EqualTo(string.Format("Could not generate derivative: {0}", -1)));
                }
            }

            public class WhenExceptionsOccurr : WhenThingsGoWrong
            {
                public override void BeforeEach()
                {
                    base.BeforeEach();
                    Exception = new Exception("testing");
                    ProcessRunner.Run(null).ReturnsForAnyArgs(x => { throw Exception; });
                    FinalException = Assert.Throws<LoggedException>(async () => await Runner.CreateDerivative(MasterFileModel, DerivativeFileModel, Arguments));
                }

                private Exception Exception { get; set; }

                [Test]
                public void ItShouldLogIssue()
                {
                    Observers.Received().LogProcessingIssue(Exception, MasterFileModel.BarCode);
                }

                [Test]
                public void ItShouldCloseExceptionCorrectly()
                {
                    Observers.Received().EndSection(Arg.Any<string>());
                }

                [Test]
                public void ItShouldThrowCorrectLoggedException()
                {
                    Assert.That(FinalException, Is.Not.Null);
                    Assert.That(FinalException.InnerException.Equals(Exception));
                }

            }








        }
    }


}