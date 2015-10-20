using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Extensions;
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

        public class WhenNormalizingOriginals : FFMPEGRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                ProductionFileModel = new ObjectFileModel(ProductionFileName);

                Originals = new List<ObjectFileModel>
                {
                    ProductionFileModel,
                    PreservationFileModel
                };

                // configure file provider to assert original should exist
                FileProvider.FileExists(Path.Combine(BaseProcessingDirectory, ProductionFileModel.GetOriginalFolderName(), ProductionFileName)).Returns(true);
                FileProvider.FileExists(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetOriginalFolderName(), MasterFileName)).Returns(true);

                // configure file provider to assert normalized versions do not exist
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, ProductionFileModel.GetFolderName(), ProductionFileName)).Returns(true);
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetFolderName(), MasterFileName)).Returns(true);
            }

            public List<ObjectFileModel> Originals { get; set; }

            private const string ProductionFileName = "MDPI_123456789_01_prod.wav";

            private ObjectFileModel ProductionFileModel { get; set; }
            private ObjectFileModel PreservationFileModel => MasterFileModel;

            public class WhenThingsGoWell : WhenNormalizingOriginals
            {
                public override async void BeforeEach()
                {
                    base.BeforeEach();

                    ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                    {
                        ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                        return Task.FromResult(ProcessRunnerResult);
                    });

                    await Runner.Normalize(Originals);
                }

                [Test]
                public void ItShouldCallProcessRunnerWithCorrectArgsForEachOriginal()
                {
                    foreach (var model in Originals)
                    {
                        var originalPath = Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFileName());
                        var targetPath = Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName());
                        var expectedArgs = $"-i {originalPath.ToQuoted()} -acodec copy -write_bext 1 -rf64 auto {targetPath.ToQuoted()}";
                        ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(i => i.Arguments.Equals(expectedArgs)));
                    }
                }

                [Test]
                public void ItShouldEndSectionForAllOriginals()
                {
                    foreach (var model in Originals)
                    {
                        Observers.Received().EndSection(Arg.Any<string>(), $"{model.ToFileName()} normalized successfully");
                    }
                }

                [Test]
                public void ItShouldOpenSectionForAllOriginals()
                {
                    foreach (var model in Originals)
                    {
                        Observers.Received().BeginSection("Normalizing {0}", model.ToFileName());
                    }
                }

                [Test]
                public void ItShouldVerifyThatAllNormalizedVersionsDoNotExist()
                {
                    foreach (var model in Originals)
                    {
                        FileProvider.Received().FileExists(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()));
                    }
                }

                [Test]
                public void ItShouldVerifyThatAllOriginalsExist()
                {
                    foreach (var model in Originals)
                    {
                        FileProvider.Received().FileDoesNotExist(Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFileName()));
                    }
                }
            }

            public class WhenThingsGoWrong : WhenNormalizingOriginals
            {
                public override async void BeforeEach()
                {
                    base.BeforeEach();

                    Originals = new List<ObjectFileModel> {PreservationFileModel};

                    // make file provider report that original does not exist
                    FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetOriginalFolderName(), MasterFileName)).Returns(true);

                    ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                    {
                        ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                        return Task.FromResult(ProcessRunnerResult);
                    });

                    await Runner.Normalize(Originals);
                }

                [Test]
                public void ItShouldCloseSectionCorrectly()
                {
                    Observers.Received().EndSection(Arg.Any<string>());
                }

                [Test]
                public void ItShouldLogIssue()
                {
                    Observers.Received().LogProcessingIssue(Arg.Any<FileNotFoundException>(), PreservationFileModel.BarCode);
                }
            }
        }

        public class WhenVerifyingNormalizedVersions : FFMPEGRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                ProductionFileModel = new ObjectFileModel(ProductionFileName);

                Originals = new List<ObjectFileModel>
                {
                    ProductionFileModel,
                    PreservationFileModel
                };

                // configure file provider to assert original should exist
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, ProductionFileModel.GetOriginalFolderName(), ProductionFileName)).Returns(false);
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetOriginalFolderName(), MasterFileName)).Returns(false);

                // configure file provider to assert normalized versions do not exist
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, ProductionFileModel.GetFolderName(), ProductionFileName)).Returns(false);
                FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetFolderName(), MasterFileName)).Returns(false);
            }

            public List<ObjectFileModel> Originals { get; set; }

            private const string ProductionFileName = "MDPI_123456789_01_prod.wav";

            private ObjectFileModel ProductionFileModel { get; set; }
            private ObjectFileModel PreservationFileModel => MasterFileModel;
            
            public class WhenThingsGoWell : WhenVerifyingNormalizedVersions
            {
                public override async void BeforeEach()
                {
                    base.BeforeEach();

                    ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                    {
                        ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                        return Task.FromResult(ProcessRunnerResult);
                    });

                    await Runner.Verify(Originals);
                }

                [Test]
                public void ItShouldOpenSectionsCorrectly()
                {
                    foreach (var model in Originals)
                    {
                        Observers.Received().BeginSection("Hashing {0}", $"{model.ToFileName()} (original)");
                        Observers.Received().BeginSection("Hashing {0}", $"{model.ToFileName()} (normalized)");
                    }
                }

                [Test]
                public void ItShouldCallFileProviderToTestThatFilesExist()
                {
                    foreach (var model in Originals)
                    {
                        FileProvider.Received().FileDoesNotExist(Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFileName()));
                        FileProvider.Received().FileDoesNotExist(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()));

                    }
                }

                [Test]
                public void ItShouldCallProcessRunnerCorrectlyForOriginalVersions()
                {
                    foreach (var model in Originals)
                    {
                        var frameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFrameMd5Filename());
                        var targetPath = Path.Combine(BaseProcessingDirectory, model.GetOriginalFolderName(), model.ToFileName());
                        
                        var expectedArgs = $"-y -i {targetPath} -f framemd5 {frameMd5Path}"; 
                        ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(i => i.Arguments.Equals(expectedArgs)));
                    }
                }

                [Test]
                public void ItShouldCallProcessRunnerCorrectlyForNormalizedVersions()
                {
                    foreach (var model in Originals)
                    {
                        var frameMd5Path = Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFrameMd5Filename());
                        var targetPath = Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName());

                        var expectedArgs = $"-y -i {targetPath} -f framemd5 {frameMd5Path}";
                        ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(i => i.Arguments.Equals(expectedArgs)));
                    }
                }


                [Test]
                public void ItShouldCloseSectionsCorrectly()
                {
                    foreach (var model in Originals)
                    {
                        Observers.Received().EndSection(Arg.Any<string>(), $"{model.ToFileName()} (original) hashed successfully");
                        Observers.Received().EndSection(Arg.Any<string>(), $"{model.ToFileName()} (normalized) hashed successfully");
                    }
                }
            }

            public class WhenThingsGoWrong : WhenVerifyingNormalizedVersions
            {
                public override async void BeforeEach()
                {
                    base.BeforeEach();

                    Originals = new List<ObjectFileModel> {PreservationFileModel};

                    // make file provider report that file does not exist
                    FileProvider.FileDoesNotExist(Path.Combine(BaseProcessingDirectory, PreservationFileModel.GetOriginalFolderName(), MasterFileName)).Returns(true);

                    ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).ReturnsForAnyArgs(x =>
                    {
                        ProcessRunnerResult.StartInfo.Returns(x.Arg<ProcessStartInfo>());
                        return Task.FromResult(ProcessRunnerResult);
                    });

                    await Runner.Verify(Originals);
                }

                [Test]
                public void ItShouldCloseSectionCorrectly()
                {
                    Observers.Received().EndSection(Arg.Any<string>());    
                }

                [Test]
                public void ItShouldLogIssueCorrectly()
                {
                    Observers.Received().LogProcessingIssue(Arg.Any<FileNotFoundException>(), PreservationFileModel.BarCode);
                }
            }
        }


        public class WhenGeneratingDerivatives : FFMPEGRunnerTests
        {
            public class WhenThingsGoWell : WhenGeneratingDerivatives
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
                        var expectedArgs =
                            $"-i {Path.Combine(BaseProcessingDirectory, MasterFileModel.GetFolderName(), MasterFileName)} {Arguments} {Path.Combine(BaseProcessingDirectory, MasterFileModel.GetFolderName(), DerivativeFileName)}";

                        ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(
                            i => i.FileName.Equals(FFMPEGPath) &&
                                 i.Arguments.Equals(expectedArgs) &&
                                 i.RedirectStandardError &&
                                 i.RedirectStandardOutput &&
                                 i.CreateNoWindow &&
                                 i.UseShellExecute == false));
                    }

                    [Test]
                    public void ItShouldLogProcessResultOutput()
                    {
                        Observers.Received().Log(StandardErrorOutput);
                    }
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
            }

            public class WhenThingsGoWrong : WhenGeneratingDerivatives
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
                        Assert.That(innerException.Message, Is.EqualTo($"Could not generate derivative: {-1}"));
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
                    public void ItShouldCloseExceptionCorrectly()
                    {
                        Observers.Received().EndSection(Arg.Any<string>());
                    }

                    [Test]
                    public void ItShouldLogIssue()
                    {
                        Observers.Received().LogProcessingIssue(Exception, MasterFileModel.BarCode);
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

        [Test]
        public void FFMPEGPathPropertyShouldHaveValidateAttribute()
        {
            var info = typeof (FFMPEGRunner).GetProperty("FFMPEGPath");
            Assert.That(info.GetCustomAttribute<ValidateFileAttribute>(), Is.Not.Null);
        }
    }
}