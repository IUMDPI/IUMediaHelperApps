﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
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
            FFMPEGRunner.CreateAccessDerivative(Arg.Any<ObjectFileModel>())
                .Returns(x => Task.FromResult(x.Arg<ObjectFileModel>().ToVideoAccessFileModel()));
            FFMPEGRunner.CreateProdOrMezzDerivative(Arg.Any<ObjectFileModel>(), Arg.Any<ObjectFileModel>(), Arg.Any<AbstractEmbeddedVideoMetadata>())
                .Returns(x => Task.FromResult(x.ArgAt<ObjectFileModel>(1)));

            SectionKey = Guid.NewGuid().ToString();
            Observers.BeginProcessingSection(Barcode, "Processing Object: {0}", Barcode).Returns(SectionKey);

            PreservationFileName = $"{ProjectCode}_{Barcode}_01_pres.mkv";
            PreservationIntermediateFileName = $"{ProjectCode}_{Barcode}_01_presInt.mkv";
            ProductionFileName = $"{ProjectCode}_{Barcode}_01_mezz.mov";
            AccessFileName = $"{ProjectCode}_{Barcode}_01_access.mp4";
            XmlManifestFileName = $"{ProjectCode}_{Barcode}.xml";

            PresObjectFileModel = new ObjectFileModel(PreservationFileName);
            PresIntObjectFileModel = new ObjectFileModel(PreservationIntermediateFileName);
            ProdObjectFileModel = new ObjectFileModel(ProductionFileName);
            AccessObjectFileModel = new ObjectFileModel(AccessFileName);

            ModelList = new List<AbstractFileModel> {PresObjectFileModel};

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

                        ModelList = new List<AbstractFileModel>
                        {
                            new ObjectFileModel(NonNormalPresFileName),
                            new ObjectFileModel(NonNormalPresIntFileName),
                            new ObjectFileModel(NonNormalProductionFileName)
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

                            ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
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

                            ModelList = new List<AbstractFileModel> {PresObjectFileModel, ProdObjectFileModel};
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
                public class WhenUnitPrefixSet : WhenGettingMetadata
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();
                        ProgramSettings.UnitPrefix.Returns("Indiana University-Bloomington. ");
                    }

                    [Test]
                    public void ItShouldUsePrefix()
                    {
                        Assert.That(Metadata.Unit.StartsWith("Indiana University-Bloomington. "), Is.True);
                    }
                }

                public class WhenUnitPrefixNotSet : WhenGettingMetadata
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();
                        ProgramSettings.UnitPrefix.Returns((string) null);
                    }

                    [Test]
                    public void ItShouldNotUsePrefix()
                    {
                        Assert.That(Metadata.Unit.StartsWith("Indiana University-Bloomington. "), Is.False);
                    }
                }

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

                [Test]
                public void ItShouldResolveUnit()
                {
                    MetadataProvider.Received().ResolveUnit(Arg.Any<string>());
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
                        FFMPEGRunner.Received().Normalize(Arg.Is<ObjectFileModel>(m=>m.IsSameAs(model.ToFileName())), Arg.Any<AbstractEmbeddedVideoMetadata>());
                    }
                }

                [Test]
                public void ItShouldVerifyAllNormalizedFiles()
                {
                    FFMPEGRunner.Received().Verify(Arg.Is<List<ObjectFileModel>>(l => l.SequenceEqual(ModelList)));
                }
            }

            public class WhenClearingMetadata : WhenNothingGoesWrong
            {
                [Test]
                public void ItShouldCallBextProcessorWithCorrectListOfFields()
                {
                    BextProcessor.Received().ClearMetadataFields(Arg.Any<List<ObjectFileModel>>(),
                        Arg.Is<List<BextFields>>(
                            a => a.SequenceEqual(new List<BextFields> {BextFields.ISFT, BextFields.ITCH})));
                }

                [Test]
                public void ItShouldCloseSection()
                {
                    Observers.Received().EndSection(Arg.Any<string>(), "Metadata fields cleared successfully");
                }

                [Test]
                public void ItShouldOpenSection()
                {
                    Observers.Received().BeginSection("Clearing metadata fields");
                }
            }

            public class WhenCreatingDerivatives : WhenNothingGoesWrong
            {
                private ObjectFileModel ExpectedMasterModel { get; set; }
                private EmbeddedAudioMetadata ExpectedMetadata { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedMasterModel = PresObjectFileModel;
                    ExpectedMetadata = new EmbeddedAudioMetadata();
                    AudioMetadataFactory.Generate(null, null, null).ReturnsForAnyArgs(ExpectedMetadata);
                }

                public class WhenPreservationIntermediateMasterPresent : WhenCreatingDerivatives
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                        ExpectedMasterModel = PresIntObjectFileModel;
                    }
                }

                [Test]
                public void ItShouldCallMetadataFactoryWithProductionMasterAsTarget()
                {
                    AudioMetadataFactory.Received()
                        .Generate(Arg.Any<List<ObjectFileModel>>(),
                            Arg.Is<ObjectFileModel>(m => m.IsProductionVersion()), Arg.Any<AudioPodMetadata>());
                }

                [Test]
                public void ItShouldCreateAccessFileFromProductionMaster()
                {
                    FFMPEGRunner.Received()
                        .CreateAccessDerivative(Arg.Is<ObjectFileModel>(m => m.IsProductionVersion()));
                }

                [Test]
                public void ItShouldCreateProdFromMasterWithExpectedMetadata()
                {
                    FFMPEGRunner.Received()
                        .CreateProdOrMezzDerivative(ExpectedMasterModel,
                            Arg.Is<ObjectFileModel>(m => m.IsProductionVersion()), ExpectedMetadata);
                }
            }

            public class WhenGeneratingXmlManifest : WhenNothingGoesWrong
            {
                private AudioCarrier AudioCarrier { get; set; }
                private int ExpectedModelCount { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    AudioCarrier = new AudioCarrier();
                    MetadataGenerator.Generate((AudioPodMetadata) null, null).ReturnsForAnyArgs(AudioCarrier);
                    ExpectedModelCount = 3; // pres master + prod master + access master
                }

                public class WhenPreservationIntermediateModelPresent : WhenGeneratingXmlManifest
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
                        ExpectedModelCount = 4; // pres master, presInt master, prod-master, access master
                    }

                    [Test]
                    public void ItShouldPassSinglePresentationIntermediateModelToGenerator()
                    {
                        MetadataGenerator.Received().Generate(
                            Arg.Any<AudioPodMetadata>(),
                            Arg.Is<List<ObjectFileModel>>(
                                l => l.SingleOrDefault(m => m.IsPreservationIntermediateVersion()) != null));
                    }
                }

                [Test]
                public void ItShouldCallExportToFileCorrectly()
                {
                    XmlExporter.Received().ExportToFile(Arg.Is<IU>(iu => iu.Carrier.Equals(AudioCarrier)),
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
                        Arg.Any<AudioPodMetadata>(),
                        Arg.Is<List<ObjectFileModel>>(l => l.Count == ExpectedModelCount));
                }

                [Test]
                public void ItShouldPassSingleAccessModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<AudioPodMetadata>(),
                        Arg.Is<List<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsAccessVersion()) != null));
                }

                [Test]
                public void ItShouldPassSinglePreservationModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<AudioPodMetadata>(),
                        Arg.Is<List<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsPreservationVersion()) != null));
                }

                [Test]
                public void ItShouldPassSingleProductionModelToGenerator()
                {
                    MetadataGenerator.Received().Generate(
                        Arg.Any<AudioPodMetadata>(),
                        Arg.Is<List<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null));
                }
            }

            public class WhenHashingFiles : WhenNothingGoesWrong
            {
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    ProcessedModelList =
                        MetadataGenerator.ReceivedCalls().First().GetArguments()[1] as List<ObjectFileModel>;
                }

                private List<ObjectFileModel> ProcessedModelList { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    Hasher.Hash(Arg.Any<ObjectFileModel>())
                        .Returns(x => Task.FromResult($"{x.Arg<ObjectFileModel>().ToFileName()} checksum"));
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
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(AccessFileName)).Checksum,
                        Is.EqualTo($"{AccessFileName} checksum"));
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoPreservationMaster()
                {
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(PreservationFileName)).Checksum,
                        Is.EqualTo($"{PreservationFileName} checksum"));
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoProductionVersion()
                {
                    Assert.That(ProcessedModelList.Single(m => m.IsSameAs(ProductionFileName)).Checksum,
                        Is.EqualTo($"{ProductionFileName} checksum"));
                }

                [Test]
                public void ItShouldCallHasherForAccessVersion()
                {
                    Hasher.Received().Hash(Arg.Is<ObjectFileModel>(m => m.IsSameAs(AccessFileName)));
                }

                [Test]
                public void ItShouldCallHasherForPreservationMaster()
                {
                    Hasher.Received().Hash(Arg.Is<ObjectFileModel>(m => m.IsSameAs(PreservationFileName)));
                }

                [Test]
                public void ItShouldCallHasherForProductionVersion()
                {
                    Hasher.Received().Hash(Arg.Is<ObjectFileModel>(m => m.IsSameAs(ProductionFileName)));
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

                        ModelList = new List<AbstractFileModel> {PresObjectFileModel, PresIntObjectFileModel};
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