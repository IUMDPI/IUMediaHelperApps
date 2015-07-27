using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Processors;

namespace Packager.Test.Processors
{
    [TestFixture]
    public abstract class AudioProcessorTests : AbstractProcessorTests
    {
        public class WhenProcessingFiles : AudioProcessorTests
        {
            private Guid SectionGuid { get; set; }

            private const string ProdCommandLineArgs = "-c:a pcm_s24le -b:a 128k -strict -2 -ar 96000";
            private const string AccessCommandLineArgs = "-c:a aac -b:a 128k -strict -2 -ar 48000";
            private const string FFMPEGPath = "ffmpeg.exe";

            protected override void DoCustomSetup()
            {
                SectionGuid = Guid.NewGuid();
                Observers.BeginSection("Processing Object: {0}", BarCode).Returns(SectionGuid);

                ProductionFileName = string.Format("{0}_{1}_01_prod.wav", ProjectCode, BarCode);
                PreservationFileName = string.Format("{0}_{1}_01_pres.wav", ProjectCode, BarCode);
                AccessFileName = string.Format("{0}_{1}_01_access.mp4", ProjectCode, BarCode);

                PresObjectFileModel = new ObjectFileModel(PreservationFileName);
                ProdObjectFileModel = new ObjectFileModel(ProductionFileName);
                AccessObjectFileModel = new ObjectFileModel(AccessFileName);

                ProcessingDirectory = string.Format("{0}_{1}", ProjectCode, BarCode);

                DependencyProvider.MetadataProvider.Get(BarCode).Returns(Task.FromResult(new ConsolidatedPodMetadata { Success = true }));

                Metadata = new ConsolidatedPodMetadata
                {
                    Barcode = BarCode,
                    Success = true
                };

                MetadataProvider.Get(BarCode).Returns(Task.FromResult(Metadata));

                Processor = new AudioProcessor(DependencyProvider);

                ProgramSettings.FFMPEGAudioAccessArguments.Returns(AccessCommandLineArgs);
                ProgramSettings.FFMPEGAudioProductionArguments.Returns(ProdCommandLineArgs);
                ProgramSettings.FFMPEGPath.Returns(FFMPEGPath);

            }
            
            public class WhenInitializing : WhenProcessingFiles
            {
                [Test]
                public void ItShouldCallBeginSectionCorrectly()
                {
                    Observers.Received().BeginSection("Processing Object: {0}", BarCode);
                }

                [Test]
                public void ItShouldCreateProcessingDirectory()
                {
                    DirectoryProvider.Received().CreateDirectory(ProcessingDirectory);
                }

                [Test]
                public void ItShouldMoveFilesToProcessingDirectory()
                {
                    FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, ProductionFileName), Path.Combine(ProcessingDirectory, ProductionFileName));
                    FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, PreservationFileName), Path.Combine(ProcessingDirectory, PreservationFileName));
                }

                [Test]
                public void ItShouldOpenInitializingSection()
                {
                    Observers.Received().BeginSection("Initializing");
                }

                [Test]
                public void ItShouldCloseInitializingSection()
                {
                    Observers.Received().EndSection(Arg.Any<Guid>(), "Initialization successful", true);
                }

            }

            public class WhenGettingMetadata : WhenProcessingFiles
            {
                [Test]
                public void ItShouldGetPodMetadata()
                {
                    MetadataProvider.Received().Get(BarCode);
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Requesting metadata for object: {0}", BarCode);
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(Arg.Any<Guid>(), string.Format("Retrieved metadata for object: {0}", BarCode), true);
                }

                [Test]
                public void ItShouldLogMetadataResults()
                {
                    Observers.Received().LogObjectProperties(Arg.Is<ConsolidatedPodMetadata>(m => m.Barcode.Equals(BarCode)));
                }
            }

            public class WhenCreatingDerivatives : WhenProcessingFiles
            {
                public class WhenProductionFileAlreadyExists : WhenCreatingDerivatives
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();
                        FileProvider.FileExists(Path.Combine(ProcessingDirectory, ProductionFileName)).Returns(true);
                    }

                    [Test]
                    public void ItShouldNotCreateDerivative()
                    {
                        AssertNotCalled(PreservationFileName, ProductionFileName, ProdCommandLineArgs);
                    }

                    [Test]
                    public void ItShouldLogDerivativeAlreadyExists()
                    {
                        Observers.Received().Log("{0} already exists. Will not generate derivate", ProdObjectFileModel.FullFileUse);
                    }
                }

                public class WhenProductionFileDoesNotExist : WhenCreatingDerivatives
                {
                    [Test]
                    public void ItShouldCreateDerivative()
                    {
                        AssertCalled(PreservationFileName, ProductionFileName, ProdCommandLineArgs);
                    }
                 }

                [Test]
                public void ItShouldCreateAccessFileCorrectly()
                {
                    AssertCalled(ProductionFileName, AccessFileName, AccessCommandLineArgs);
                }

                [Test]
                public void ItShouldCloseProductionSection()
                {
                    var expected = string.Format("{0} generated successfully: {1}", ProdObjectFileModel.FullFileUse, ProductionFileName);
                    Observers.Received().EndSection(Arg.Any<Guid>(), expected, true);
                }

                [Test]
                public void ItShouldOpenProductionSection()
                {
                    Observers.Received().BeginSection("Generating {0}: {1}", ProdObjectFileModel.FullFileUse, ProductionFileName);
                }
                
                [Test]
                public void ItShouldOpenAccessSection()
                {
                    Observers.Received().BeginSection("Generating {0}: {1}", AccessObjectFileModel.FullFileUse, AccessFileName);
                }

                [Test]
                public void ItShouldCloseAccessSection()
                {
                    var expected = string.Format("{0} generated successfully: {1}", AccessObjectFileModel.FullFileUse, AccessFileName);
                    Observers.Received().EndSection(Arg.Any<Guid>(), expected, true);
                }

                private void AssertCalled(string originalFileName, string newFileName, string settingsArgs)
                {
                    var expectedArgs = string.Format("-i {0} {1} {2}",
                        Path.Combine(ProcessingDirectory, originalFileName),
                        settingsArgs,
                        Path.Combine(ProcessingDirectory, newFileName));

                    ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(
                        i => i.FileName.Equals(FFMPEGPath) &&
                             i.Arguments.Equals(expectedArgs) &&
                             i.RedirectStandardError &&
                             i.RedirectStandardOutput &&
                             i.CreateNoWindow &&
                             i.UseShellExecute == false));
                }

                private void AssertNotCalled(string originalFileName, string newFileName, string settingsArgs)
                {
                    var expectedArgs = string.Format("-i {0} {1} {2}",
                        Path.Combine(ProcessingDirectory, originalFileName),
                        settingsArgs,
                        Path.Combine(ProcessingDirectory, newFileName));

                    ProcessRunner.DidNotReceive().Run(Arg.Is<ProcessStartInfo>(
                        i => i.FileName.Equals(FFMPEGPath) &&
                             i.Arguments.Equals(expectedArgs) &&
                             i.RedirectStandardError &&
                             i.RedirectStandardOutput &&
                             i.CreateNoWindow &&
                             i.UseShellExecute == false));
                }
            }

            public class WhenEmbeddingMetadata : WhenProcessingFiles
            {
                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Adding BEXT metadata");
                }
                


               

            }

            public class WhenFinalizing : WhenProcessingFiles
            {
                public class IfSuccessful : WhenFinalizing
                {
                    [Test]
                    public void ItShouldCopyFilesToDropbox()
                    {
                    }

                    [Test]
                    public void ItShouldMoveFilesToSuccessFolder()
                    {
                    }

                    [Test]
                    public void ItShouldCallEndSectionCorrectly()
                    {
                        Observers.Received().EndSection(SectionGuid, "Object processed succesfully: 4890764553278906", true);
                    }
                }

                public class IfIssueOccurs : WhenFinalizing
                {

                }
            }





        }

       
    }
}