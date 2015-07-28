using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Processors;

namespace Packager.Test.Processors
{
    [TestFixture]
    public class AudioProcessorTests : AbstractProcessorTests
    {
        private const string ProdCommandLineArgs = "-c:a pcm_s24le -b:a 128k -strict -2 -ar 96000";
        private const string AccessCommandLineArgs = "-c:a aac -b:a 128k -strict -2 -ar 48000";
        private const string FFMPEGPath = "ffmpeg.exe";
        private Guid SectionGuid { get; set; }

        protected override void DoCustomSetup()
        {
            SectionGuid = Guid.NewGuid();
            Observers.BeginSection("Processing Object: {0}", Barcode).Returns(SectionGuid);

            PreservationFileName = string.Format("{0}_{1}_01_pres.wav", ProjectCode, Barcode);
            PreservationIntermediateFileName = string.Format("{0}_{1}_01_pres-int.wav", ProjectCode, Barcode);
            ProductionFileName = string.Format("{0}_{1}_01_prod.wav", ProjectCode, Barcode);
            AccessFileName = string.Format("{0}_{1}_01_access.mp4", ProjectCode, Barcode);
            XmlManifestFileName = string.Format("{0}_{1}.xml", ProjectCode, Barcode);

            PresObjectFileModel = new ObjectFileModel(PreservationFileName);
            PresIntObjectFileModel = new ObjectFileModel(PreservationIntermediateFileName);
            ProdObjectFileModel = new ObjectFileModel(ProductionFileName);
            AccessObjectFileModel = new ObjectFileModel(AccessFileName);

            ModelList = new List<AbstractFileModel> {PresObjectFileModel};

            ExpectedObjectFolderName = string.Format("{0}_{1}", ProjectCode, Barcode);

            DependencyProvider.MetadataProvider.Get(Barcode).Returns(Task.FromResult(new ConsolidatedPodMetadata {Success = true}));

            Metadata = new ConsolidatedPodMetadata
            {
                Barcode = Barcode,
                Success = true
            };

            MetadataProvider.Get(Barcode).Returns(Task.FromResult(Metadata));

            Processor = new AudioProcessor(DependencyProvider);

            ProgramSettings.FFMPEGAudioAccessArguments.Returns(AccessCommandLineArgs);
            ProgramSettings.FFMPEGAudioProductionArguments.Returns(ProdCommandLineArgs);
            ProgramSettings.FFMPEGPath.Returns(FFMPEGPath);
        }

        public class WhenNothingGoesWrong : AudioProcessorTests
        {
            [Test]
            public void ProcessorShouldReturnTrue()
            {
                Assert.That(Result, Is.EqualTo(true));
            }

            public class WhenInitializing : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldCallBeginSectionCorrectly()
                {
                    Observers.Received().BeginSection("Processing Object: {0}", Barcode);
                }

                [Test]
                public void ItShouldCreateProcessingDirectory()
                {
                    DirectoryProvider.Received().CreateDirectory(ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldMovePresentationFileToProcessingDirectory()
                {
                    FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, PreservationFileName), Path.Combine(ExpectedProcessingDirectory, PreservationFileName));
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

                public class WhenPreservationIntermediateMasterPresent : WhenInitializing
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                    }

                    [Test]
                    public void ItShouldMovePreservationIntermediateMasterToProcessingDirectory()
                    {
                        FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, PreservationIntermediateFileName),
                            Path.Combine(ExpectedProcessingDirectory, PreservationIntermediateFileName));
                    }
                }

                public class WhenProductionMasterPresent : WhenInitializing
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, ProdObjectFileModel};
                    }

                    [Test]
                    public void ItShouldMovePreservationIntermediateMasterToProcessingDirectory()
                    {
                        FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, ProductionFileName),
                            Path.Combine(ExpectedProcessingDirectory, ProductionFileName));
                    }
                }
            }

            public class WhenGettingMetadata : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldGetPodMetadata()
                {
                    MetadataProvider.Received().Get(Barcode);
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Requesting metadata for object: {0}", Barcode);
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(Arg.Any<Guid>(), string.Format("Retrieved metadata for object: {0}", Barcode), true);
                }

                [Test]
                public void ItShouldLogMetadataResults()
                {
                    Observers.Received().LogObjectProperties(Arg.Is<ConsolidatedPodMetadata>(m => m.Barcode.Equals(Barcode)));
                }
            }

            public class WhenCreatingDerivatives : WhenNothingGoesWrong
            {
                private string ExpectedMasterFileName { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedMasterFileName = PreservationFileName;
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
                        Path.Combine(ExpectedProcessingDirectory, originalFileName),
                        settingsArgs,
                        Path.Combine(ExpectedProcessingDirectory, newFileName));

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
                        Path.Combine(ExpectedProcessingDirectory, originalFileName),
                        settingsArgs,
                        Path.Combine(ExpectedProcessingDirectory, newFileName));

                    ProcessRunner.DidNotReceive().Run(Arg.Is<ProcessStartInfo>(
                        i => i.FileName.Equals(FFMPEGPath) &&
                             i.Arguments.Equals(expectedArgs) &&
                             i.RedirectStandardError &&
                             i.RedirectStandardOutput &&
                             i.CreateNoWindow &&
                             i.UseShellExecute == false));
                }

                public class WhenProductionFileAlreadyExists : WhenCreatingDerivatives
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, ProdObjectFileModel};
                        FileProvider.FileExists(Path.Combine(ExpectedProcessingDirectory, ProductionFileName)).Returns(true);
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
                    public void ItShouldCreateDerivativeFromExpectedMaster()
                    {
                        AssertCalled(ExpectedMasterFileName, ProductionFileName, ProdCommandLineArgs);
                    }

                    public class WhenPreservationIntermediateMasterPresent : WhenProductionFileDoesNotExist
                    {
                        protected override void DoCustomSetup()
                        {
                            base.DoCustomSetup();

                            ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                            ExpectedMasterFileName = PreservationIntermediateFileName;
                        }
                    }
                }
            }

            public class WhenEmbeddingMetadata : WhenNothingGoesWrong
            {
                private int ExpectedModelCount { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    ExpectedModelCount = 2; // pres master + prod master
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Adding BEXT metadata");
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(Arg.Any<Guid>(), "BEXT metadata added successfully", true);
                }

                [Test]
                public void ItShouldPassCorrectNumberOfObjectsToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Count() == ExpectedModelCount),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleProductionModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSinglePresentationModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldNotPassAccessModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Any(m => m.IsAccessVersion()) == false),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ExpectedProcessingDirectory);
                }

                public class WhenPreservationIntermediateModelPresent : WhenEmbeddingMetadata
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                        ExpectedModelCount = 3; // pres master, pres-int master, prod-master
                    }

                    [Test]
                    public void ItShouldPassSinglePresentationIntermediateModelToBextProcessor()
                    {
                        BextProcessor.Received().EmbedBextMetadata(
                            Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsPreservationIntermediateVersion()) != null),
                            Arg.Any<ConsolidatedPodMetadata>(),
                            ExpectedProcessingDirectory);
                    }
                }
            }

            public class WhenGeneratingXmlManifest : WhenNothingGoesWrong
            {
                private CarrierData CarrierData { get; set; }
                private int ExpectedModelCount { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    CarrierData = new CarrierData();
                    MetadataGenerator.GenerateMetadata(null, null, "").ReturnsForAnyArgs(CarrierData);
                    ExpectedModelCount = 3; // pres master + prod master + access master
                }

                [Test]
                public void ItShouldPassExpectedNumberOfObjectsToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Count() == ExpectedModelCount),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleProductionModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSinglePreservationModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsPreservationVersion()) != null),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleAccessModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsAccessVersion()) != null),
                        ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldCallExportToFileCorrectly()
                {
                    XmlExporter.Received().ExportToFile(Arg.Is<IU>(iu => iu.Carrier.Equals(CarrierData)),
                        Path.Combine(ExpectedProcessingDirectory, XmlManifestFileName));
                }

                public class WhenPreservationIntermediateModelPresent : WhenGeneratingXmlManifest
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                        ExpectedModelCount = 4; // pres master, pres-int master, prod-master, access master
                    }

                    [Test]
                    public void ItShouldPassSinglePresentationIntermediateModelToGenerator()
                    {
                        MetadataGenerator.Received().GenerateMetadata(
                            Arg.Any<ConsolidatedPodMetadata>(),
                            Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsPreservationIntermediateVersion()) != null),
                            ExpectedProcessingDirectory);
                    }
                }
            }

            public class WhenCopyingFilesToDropBoxFolder : WhenNothingGoesWrong
            {
                private int ExpectedFiles { get; set; }
                
                public class WhenPreservationIntermediateModelPresent : WhenCopyingFilesToDropBoxFolder
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> { PresObjectFileModel, PresIntObjectFileModel };
                        ExpectedFiles = 5; // prod master, pres master, pres-int master, access, xml manifest
                    }

                    [Test]
                    public void ItShouldCopyPreservationIntermidateMasterToDropbox()
                    {
                        var sourcePath = Path.Combine(ExpectedProcessingDirectory, PreservationIntermediateFileName);
                        var targetPath = Path.Combine(ExpectedDropboxDirectory, PreservationIntermediateFileName);
                        FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                    }


                    [Test]
                    public void ItShouldLogPreservationIntermediateMasterCopied()
                    {
                        Observers.Received().Log("copying {0} to {1}", PreservationIntermediateFileName, ExpectedDropboxDirectory);
                    }
                }
                
                private string ExpectedDropboxDirectory { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedDropboxDirectory = Path.Combine(DropBoxRoot, string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode));
                    ExpectedFiles = 4; // prod master, pres master, access, xml manifest
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Copying objects to dropbox");
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(
                        Arg.Any<Guid>(),
                        string.Format("{0} files copied to dropbox folder successfully", Barcode),
                        true);
                }

                [Test]
                public void ItShouldCreateFolderInDropBox()
                {
                    DirectoryProvider.Received().CreateDirectory(ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldCopyCorrectNumberOfFilesToDropBox()
                {
                    FileProvider.Received(ExpectedFiles).CopyFileAsync(Arg.Any<string>(), Arg.Any<string>());
                }

                [Test]
                public void ItShouldCopyPreservationMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, PreservationFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, PreservationFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldLogPreservationMasterCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", PreservationFileName, ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldLogProductionMasterCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", PreservationFileName, ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldLogAccessMasterCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", AccessFileName, ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldLogXmlManifestCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", XmlManifestFileName, ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldCopyProductionMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, ProductionFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, ProductionFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCopyAccessMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, AccessFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, AccessFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCopyXmlManifestToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, XmlManifestFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, XmlManifestFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }
            }

            public class WhenFinalizing : WhenNothingGoesWrong
            {
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
        }

        public class WhenThingsGoWrong : AudioProcessorTests
        {
            private string IssueMessage { get; set; }
            private Guid SubSectionGuid { get; set; }

            public abstract class CommonTests : WhenThingsGoWrong
            {
                [Test]
                public void ProcessorShouldReturnFalse()
                {
                    Assert.That(Result, Is.EqualTo(false));
                }
                
                [Test]
                public void ShouldLogException()
                {
                    Observers.Received().LogIssue(Arg.Is<Exception>(e => e.Message.Equals(IssueMessage)));
                }

                [Test]
                public void ShouldMoveContentToErrorFolder()
                {
                    DirectoryProvider.Received().MoveDirectory(ExpectedProcessingDirectory, Path.Combine(ErrorDirectory, ExpectedObjectFolderName));
                }

                [Test]
                public void ShouldLogMovingMessage()
                {
                    Observers.Received().Log("Moving {0} to error directory", ExpectedObjectFolderName);
                }

                [Test]
                public void ShouldCloseProcessingSection()
                {
                    Observers.Received().EndSection(SectionGuid);
                }

                [Test]
                public void ShouldCloseSubSection()
                {
                    if (SubSectionGuid == Guid.Empty)
                    {
                        return;
                    }

                    Observers.Received().EndSection(SubSectionGuid);
                }
            }

            public class WhenCreateProcessingDirectoryFails : CommonTests
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    IssueMessage = "Create processing directory failed";
                    DirectoryProvider.CreateDirectory(ExpectedProcessingDirectory).Returns(x=> { throw new Exception(IssueMessage); });

                }
            }

            public class WhenCopyingToDropBoxFails : CommonTests
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    SubSectionGuid = Guid.NewGuid();
                    Observers.BeginSection("Copying objects to dropbox").Returns(SubSectionGuid);
                    IssueMessage = "dropbox operation failed";
                    FileProvider.CopyFileAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(x => { throw new Exception(IssueMessage); });
                }

                [Test]
                public void ShouldThrowLoggedException()
                {
                    Observers.Received().LogIssue(Arg.Is<LoggedException>(e=>e.InnerException.Message.Equals(IssueMessage)));
                }
            }
        }
    }
}