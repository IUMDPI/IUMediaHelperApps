using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;
using Packager.Processors;
using Packager.Utilities.Bext;

namespace Packager.Test.Processors
{
    [TestFixture]
    public class VideoProcessorTests : AbstractProcessorTests
    {
        private const string ProdCommandLineArgs = "-c:a pcm_s24le -b:a 128k -strict -2 -ar 96000";
        private const string AccessCommandLineArgs = "-c:a aac -b:a 128k -strict -2 -ar 48000";
        private const string FFMPEGPath = "ffmpeg.exe";
        private string SectionKey { get; set; }

        private VideoPodMetadata Metadata { get; set; }

        protected override void DoCustomSetup()
        {
            FFMPEGRunner.CreateAccessDerivative(Arg.Any<AbstractFile>())
                .Returns(x => Task.FromResult((AbstractFile) new AccessFile(x.Arg<AbstractFile>())));
            FFMPEGRunner.CreateProdOrMezzDerivative(Arg.Any<AbstractFile>(), Arg.Any<AbstractFile>(), Arg.Any<AbstractEmbeddedMetadata>())
                .Returns(x => Task.FromResult(x.ArgAt<AbstractFile>(1)));
            FFMPEGRunner.AccessArguments.Returns("Test");
            //FFMPEGRunner.CreateProdOrMezzDerivative(Arg.Any<AbstractFile>(), Arg.Any<AbstractFile>(), Arg.Any<EmbeddedVideoMezzanineMetadata>())
            //    .Returns(x => Task.FromResult(x.ArgAt<AbstractFile>(1)));

            SectionKey = Guid.NewGuid().ToString();
            Observers.BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode).Returns(SectionKey);

            PreservationFileName = $"{ProjectCode}_{Barcode}_01_pres.mkv";
            PreservationIntermediateFileName = $"{ProjectCode}_{Barcode}_01_presInt.mkv";
            ProductionFileName = $"{ProjectCode}_{Barcode}_01_mezz.mov";
            AccessFileName = $"{ProjectCode}_{Barcode}_01_access.mp4";
            XmlManifestFileName = $"{ProjectCode}_{Barcode}.xml";

            PresAbstractFile = FileModelFactory.GetModel(PreservationFileName);
            PresIntAbstractFile = FileModelFactory.GetModel(PreservationIntermediateFileName);
            ProdAbstractFile = FileModelFactory.GetModel(ProductionFileName);
            AccessAbstractFile = FileModelFactory.GetModel(AccessFileName);

            ModelList = new List<AbstractFile> {PresAbstractFile};

            ExpectedObjectFolderName = $"{ProjectCode}_{Barcode}";

            Metadata = new VideoPodMetadata {Barcode = Barcode};

            MetadataProvider.GetObjectMetadata<VideoPodMetadata>(Barcode).Returns(Task.FromResult(Metadata));

            Processor = new VideoProcessor(DependencyProvider);

            ProgramSettings.FFMPEGAudioAccessArguments.Returns(AccessCommandLineArgs);
            ProgramSettings.FFMPEGAudioProductionArguments.Returns(ProdCommandLineArgs);
            ProgramSettings.FFMPEGPath.Returns(FFMPEGPath);
        }

        public class WhenNothingGoesWrong : VideoProcessorTests
        {
            public class WhenInitializing : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldCallBeginSectionCorrectly()
                {
                    Observers.Received().BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode);
                }

                [Test]
                public void ItShouldCloseInitializingSection()
                {
                    Observers.Received().EndSection(Arg.Any<string>(), "Initialization successful");
                }

                [Test]
                public void ItShouldCreateOriginalsDirectory()
                {
                    DirectoryProvider.Received().CreateDirectory(ExpectedOriginalsDirectory);
                }

                [Test]
                public void ItShouldCreateProcessingDirectory()
                {
                    DirectoryProvider.Received().CreateDirectory(ExpectedProcessingDirectory);
                }

                [Test]
                public void ItShouldOpenInitializingSection()
                {
                    Observers.Received().BeginSection("Initializing");
                }
            }

            public class WhenMovingFilesToProcessingDirectory : WhenNothingGoesWrong
            {
                public class WhenFileNamesNotNormalized : WhenMovingFilesToProcessingDirectory
                {
                    private string NonNormalPresFileName { get; set; }
                    private string NonNormalPresIntFileName { get; set; }
                    private string NonNormalProductionFileName { get; set; }

                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        NonNormalPresFileName = $"{ProjectCode.ToLowerInvariant()}_{Barcode}_1_pres.mkv";
                        NonNormalPresIntFileName = $"{ProjectCode.ToLowerInvariant()}_{Barcode}_001_presInt.mkv";
                        NonNormalProductionFileName = $"{ProjectCode.ToLowerInvariant()}_{Barcode}_01_mezz.mov";

                        ModelList = new List<AbstractFile>
                        {
                            FileModelFactory.GetModel(NonNormalPresFileName),
                            FileModelFactory.GetModel(NonNormalPresIntFileName),
                            FileModelFactory.GetModel(NonNormalProductionFileName)
                        };
                    }

                    [Test]
                    public void ItShouldNormalizeFileNamesOnMove()
                    {
                        FileProvider.Received().MoveFileAsync(
                            Path.Combine(InputDirectory, NonNormalPresFileName),
                            Path.Combine(ExpectedOriginalsDirectory, PreservationFileName));

                        FileProvider.Received().MoveFileAsync(
                            Path.Combine(InputDirectory, NonNormalPresIntFileName),
                            Path.Combine(ExpectedOriginalsDirectory, PreservationIntermediateFileName));

                        FileProvider.Received().MoveFileAsync(
                            Path.Combine(InputDirectory, NonNormalProductionFileName),
                            Path.Combine(ExpectedOriginalsDirectory, ProductionFileName));
                    }
                }

                public class WhenFileNamesAreNormal : WhenMovingFilesToProcessingDirectory
                {
                    public class WhenPreservationIntermediateMasterPresent : WhenMovingFilesToProcessingDirectory
                    {
                        protected override void DoCustomSetup()
                        {
                            base.DoCustomSetup();

                            ModelList = new List<AbstractFile> {PresAbstractFile, PresIntAbstractFile};
                        }

                        [Test]
                        public void ItShouldMovePreservationIntermediateMasterToOriginalsDirectory()
                        {
                            FileProvider.Received()
                                .MoveFileAsync(Path.Combine(InputDirectory, PreservationIntermediateFileName),
                                    Path.Combine(ExpectedOriginalsDirectory, PreservationIntermediateFileName));
                        }
                    }

                    public class WhenProductionMasterPresent : WhenMovingFilesToProcessingDirectory
                    {
                        protected override void DoCustomSetup()
                        {
                            base.DoCustomSetup();

                            ModelList = new List<AbstractFile> {PresAbstractFile, ProdAbstractFile};
                        }

                        [Test]
                        public void ItShouldMovePreservationIntermediateMasterToOriginalsDirectory()
                        {
                            FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, ProductionFileName),
                                Path.Combine(ExpectedOriginalsDirectory, ProductionFileName));
                        }
                    }

                    [Test]
                    public void ItShouldMovePresentationFileToOriginalsDirectory()
                    {
                        FileProvider.Received()
                            .MoveFileAsync(Path.Combine(InputDirectory, PreservationFileName),
                                Path.Combine(ExpectedOriginalsDirectory, PreservationFileName));
                    }
                }
            }

            public class WhenGettingMetadata : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(Arg.Any<string>(), $"Retrieved metadata for object: {Barcode}");
                }

                [Test]
                public void ItShouldGetPodMetadata()
                {
                    MetadataProvider.Received().GetObjectMetadata<VideoPodMetadata>(Barcode);
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Requesting metadata for object: {0}", Barcode);
                }
            }

            public class WhenNormalizingOriginals : WhenNothingGoesWrong
            {
                private EmbeddedVideoPreservationMetadata ExpectedMetadata { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedMetadata = new EmbeddedVideoPreservationMetadata();
                    VideoMetadataFactory.Generate(null, null, null).ReturnsForAnyArgs(ExpectedMetadata);
                }


                [Test]
                public void ItShouldNormalizeAllOriginalFilesWithExpectedMetadata()
                {
                    foreach (var model in ModelList)
                    {
                        FFMPEGRunner.Received().Normalize(Arg.Is<AbstractFile>(m=>m.ToFileName().Equals(model.ToFileName())), 
                            Arg.Any<AbstractEmbeddedVideoMetadata>());
                    }
                }

                [Test]
                public void ItShouldVerifyAllNormalizedFiles()
                {
                    FFMPEGRunner.Received().Verify(Arg.Is<List<AbstractFile>>(l => l.SequenceEqual(ModelList)));
                }
            }
            
            public class WhenCreatingDerivatives : WhenNothingGoesWrong
            {
                private AbstractFile ExpectedMasterModel { get; set; }
                private AbstractEmbeddedVideoMetadata ExpectedMetadata { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedMasterModel = PresAbstractFile;
                    ExpectedMetadata = new EmbeddedVideoMezzanineMetadata();
                    VideoMetadataFactory.Generate(null, null, null).ReturnsForAnyArgs(ExpectedMetadata);
                }

                public class WhenPreservationIntermediateMasterPresent : WhenCreatingDerivatives
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFile> {PresAbstractFile, PresIntAbstractFile};
                        ExpectedMasterModel = PresIntAbstractFile;
                    }
                }

                [Test]
                public void ItShouldCallMetadataFactoryWithMezzanineMasterAsTarget()
                {
                    VideoMetadataFactory.Received()
                        .Generate(Arg.Any<List<AbstractFile>>(),
                            Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()), Arg.Any<VideoPodMetadata>());
                }

                [Test]
                public void ItShouldCreateAccessFileFromMezzanineMaster()
                {
                    FFMPEGRunner.Received()
                        .CreateAccessDerivative(Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()));
                }

                [Test]
                public void ItShouldCreateProdFromMasterWithExpectedMetadata()
                {
                    FFMPEGRunner.Received()
                        .CreateProdOrMezzDerivative(ExpectedMasterModel,
                            Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()), ExpectedMetadata);
                }
            }

            public class WhenGeneratingXmlManifest : WhenNothingGoesWrong
            {
                private VideoCarrier VideoCarrier { get; set; }
                private int ExpectedModelCount { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    VideoCarrier = new VideoCarrier();
                    MetadataGenerator.Generate((VideoPodMetadata) null, null).ReturnsForAnyArgs(VideoCarrier);
                    ExpectedModelCount = 4; // pres master + prod master + access master, qc file
                }

                public class WhenPreservationIntermediateModelPresent : WhenGeneratingXmlManifest
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFile> {PresAbstractFile, PresIntAbstractFile};
                        ExpectedModelCount = 5; // pres master, presInt master, prod-master, access master, qc file
                    }

                    [Test]
                    public void ItShouldPassSinglePresentationIntermediateModelToGenerator()
                    {
                        MetadataGenerator.Received().Generate(
                            Arg.Any<VideoPodMetadata>(),
                            Arg.Is<List<AbstractFile>>(
                                l => l.SingleOrDefault(m => m.IsPreservationIntermediateVersion()) != null));
                    }
                }

                [Test]
                public void ItShouldCallExportToFileCorrectly()
                {
                    XmlExporter.Received().ExportToFile(Arg.Is<IU>(iu => iu.Carrier.Equals(VideoCarrier)),
                        Path.Combine(ExpectedProcessingDirectory, XmlManifestFileName));
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received()
                        .EndSection(Arg.Any<string>(), $"{ProjectCode}_{Barcode}.xml generated successfully");
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Generating {0}", $"{ProjectCode}_{Barcode}.xml");
                }

                [Test]
                public void ItShouldPassExpectedNumberOfObjectsToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Is<List<AbstractFile>>(l => l.Count == ExpectedModelCount));
                }

                [Test]
                public void ItShouldPassSingleAccessModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Is<List<AbstractFile>>(l => l.SingleOrDefault(m => m.IsAccessVersion()) != null));
                }

                [Test]
                public void ItShouldPassSinglePreservationModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Is<List<AbstractFile>>(l => l.SingleOrDefault(m => m.IsPreservationVersion()) != null));
                }

                [Test]
                public void ItShouldPassSingleMezzanineModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Is<List<AbstractFile>>(l => l.SingleOrDefault(m => m.IsMezzanineVersion()) != null));
                }

                [Test]
                public void ItShouldPassSingleQualityControlModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Is<List<AbstractFile>>(l => l.SingleOrDefault(m => m is QualityControlFile) != null));
                }
            }

            public class WhenHashingFiles : WhenNothingGoesWrong
            {
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    ProcessedModelList =
                        MetadataGenerator.ReceivedCalls().First().GetArguments()[1] as List<AbstractFile>;
                }

                private List<AbstractFile> ProcessedModelList { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    Hasher.Hash(Arg.Any<AbstractFile>())
                        .Returns(x => Task.FromResult($"{x.Arg<AbstractFile>().ToFileName()} checksum"));
                }

                [TestCase]
                public void ItShouldLogChecksumsCorrectly()
                {
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(PreservationFileName),
                            $"{PreservationFileName} checksum");
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(ProductionFileName),
                            $"{ProductionFileName} checksum");
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(AccessFileName),
                            $"{AccessFileName} checksum");
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoAccessVersion()
                {
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(new UnknownFile(AccessFileName))).Checksum,
                        Is.EqualTo($"{AccessFileName} checksum"));
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoPreservationMaster()
                {
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(new UnknownFile(PreservationFileName))).Checksum,
                        Is.EqualTo($"{PreservationFileName} checksum"));
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoProductionVersion()
                {
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(new UnknownFile(ProductionFileName))).Checksum,
                        Is.EqualTo($"{ProductionFileName} checksum"));
                }

                [Test]
                public void ItShouldCallHasherForAccessVersion()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(AccessFileName))));
                }

                [Test]
                public void ItShouldCallHasherForPreservationMaster()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(PreservationFileName))));
                }

                [Test]
                public void ItShouldCallHasherForProductionVersion()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(ProductionFileName))));
                }
            }

            public class WhenCopyingFilesToDropBoxFolder : WhenNothingGoesWrong
            {
                private int ExpectedFiles { get; set; }
                private string ExpectedDropboxDirectory { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedDropboxDirectory = Path.Combine(DropBoxRoot, $"{ProjectCode.ToUpperInvariant()}_{Barcode}");
                    ExpectedFiles = 4; // prod master, pres master, access, xml manifest
                }

                public class WhenPreservationIntermediateModelPresent : WhenCopyingFilesToDropBoxFolder
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFile> {PresAbstractFile, PresIntAbstractFile};
                        ExpectedFiles = 5; // prod master, pres master, presInt master, access, xml manifest
                    }

                    [Test]
                    public void ItShouldCopyPreservationIntermediateMasterToDropbox()
                    {
                        var sourcePath = Path.Combine(ExpectedProcessingDirectory, PreservationIntermediateFileName);
                        var targetPath = Path.Combine(ExpectedDropboxDirectory, PreservationIntermediateFileName);
                        FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                    }

                    [Test]
                    public void ItShouldLogPreservationIntermediateMasterCopied()
                    {
                        Observers.Received()
                            .Log("copying {0} to {1}", PreservationIntermediateFileName, ExpectedDropboxDirectory);
                    }
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(
                        Arg.Any<string>(),
                        $"{Barcode} files copied to dropbox folder successfully");
                }

                [Test]
                public void ItShouldCopyAccessMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, AccessFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, AccessFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
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
                public void ItShouldCopyProductionMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, ProductionFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, ProductionFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCopyXmlManifestToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, XmlManifestFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, XmlManifestFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCreateFolderInDropBox()
                {
                    DirectoryProvider.Received().CreateDirectory(ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldLogAccessMasterCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", AccessFileName, ExpectedDropboxDirectory);
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
                public void ItShouldLogXmlManifestCopied()
                {
                    Observers.Received().Log("copying {0} to {1}", XmlManifestFileName, ExpectedDropboxDirectory);
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Copying objects to dropbox");
                }
            }

            public class WhenFinalizing : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldCallEndSectionCorrectly()
                {
                    Observers.Received().EndSection(SectionKey, "Object processed succesfully: 4890764553278906");
                }

                [Test]
                public void ItShouldMoveFilesToSuccessFolder()
                {
                }
            }

            [Test]
            public void ProcessorShouldReturnTrue()
            {
                Assert.That(Result.Result, Is.EqualTo(true));
            }
        }

        public class WhenThingsGoWrong : VideoProcessorTests
        {
            private string IssueMessage { get; set; }
            private string SubSectionKey { get; set; }

            public abstract class CommonTests : WhenThingsGoWrong
            {
                [Test]
                public void ProcessorShouldReturnFalse()
                {
                    Assert.That(Result.Result, Is.EqualTo(false));
                }

                [Test]
                public void ShouldCloseProcessingSection()
                {
                    Observers.Received().EndSection(SectionKey);
                }

                [Test]
                public void ShouldCloseSubSection()
                {
                    if (string.IsNullOrWhiteSpace(SubSectionKey))
                    {
                        return;
                    }

                    Observers.Received().EndSection(SubSectionKey);
                }

                [Test]
                public void ShouldLogException()
                {
                    Observers.Received()
                        .LogProcessingIssue(Arg.Is<Exception>(e => e.Message.Equals(IssueMessage)), Barcode);
                }

                [Test]
                public void ShouldLogMovingMessage()
                {
                    Observers.Received().Log("Moving {0} to error folder", ExpectedObjectFolderName);
                }

                [Test]
                public void ShouldMoveContentToErrorFolder()
                {
                    DirectoryProvider.Received()
                        .MoveDirectory(ExpectedProcessingDirectory,
                            Path.Combine(ErrorDirectory, ExpectedObjectFolderName));
                }
            }

            public class WhenCreateProcessingDirectoryFails : CommonTests
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    IssueMessage = "Create processing directory failed";
                    DirectoryProvider.CreateDirectory(ExpectedProcessingDirectory)
                        .Returns(x => { throw new Exception(IssueMessage); });
                }
            }

            public class WhenCopyingToDropBoxFails : CommonTests
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();
                    SubSectionKey = Guid.NewGuid().ToString();
                    Observers.BeginSection("Copying objects to dropbox").Returns(SubSectionKey);
                    IssueMessage = "dropbox operation failed";
                    FileProvider.CopyFileAsync(Arg.Any<string>(), Arg.Any<string>())
                        .Returns(x => { throw new Exception(IssueMessage); });
                }

                [Test]
                public void ShouldThrowLoggedException()
                {
                    Observers.Received()
                        .LogProcessingIssue(
                            Arg.Is<LoggedException>(e => e.InnerException.Message.Equals(IssueMessage)), Barcode);
                }
            }
        }
    }
}