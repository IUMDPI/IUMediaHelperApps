﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
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
using Packager.Providers;
using Packager.Utilities.ProcessRunners;

namespace Packager.Test.Processors
{
    [TestFixture]
    public class VideoProcessorTests : AbstractProcessorTests
    {
        private const string MezzCommandLineArgs = "-c:a pcm_s24le -b:a 128k -strict -2 -ar 96000";
        private const string AccessCommandLineArgs = "-c:a aac -b:a 128k -strict -2 -ar 48000";
        private const string FFMPEGPath = "ffmpeg.exe";
        private string SectionKey { get; set; }

        private static ArgumentBuilder NormalizingArguments => new ArgumentBuilder("normalizing arguments");
        private static ArgumentBuilder MezzArguments => new ArgumentBuilder(MezzCommandLineArgs);
        private static ArgumentBuilder AccessArguments => new ArgumentBuilder(AccessCommandLineArgs);


        private MediaInfo OneStreamMediaInfo = new MediaInfo
        {
            AudioStreams = 1,
            VideoStreams = 2
        };

        private MediaInfo MultiStreamMediaInfo = new MediaInfo
        {
            AudioStreams = 2,
            VideoStreams = 2
        };

        private VideoPodMetadata Metadata { get; set; }
        private IMediaInfoProvider MediaInfoProvider { get; set; }

        protected override void DoCustomSetup()
        {
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

            ModelList = new List<AbstractFile> { PresAbstractFile };

            ExpectedObjectFolderName = $"{ProjectCode}_{Barcode}";

            Metadata = new VideoPodMetadata { Barcode = Barcode, Unit = "Unit value" };

            MetadataProvider.GetObjectMetadata<VideoPodMetadata>(Barcode, Arg.Any<CancellationToken>()).Returns(Task.FromResult(Metadata));

            FFMPEGArgumentsFactory.GetNormalizingArguments(Arg.Any<IMediaFormat>()).Returns(NormalizingArguments);
            FFMPEGArgumentsFactory.GetAccessArguments(Arg.Any<IMediaFormat>())
                .Returns(AccessArguments);
            FFMPEGArgumentsFactory.GetProdOrMezzArguments(Arg.Any<IMediaFormat>())
                .Returns(MezzArguments);

            MediaInfoProvider = Substitute.For<IMediaInfoProvider>();
            MediaInfoProvider.GetMediaInfo(Arg.Any<AbstractFile>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(MultiStreamMediaInfo));

            Processor = new VideoProcessor(BextProcessor, DirectoryProvider, FileProvider, Hasher,
                MetadataProvider, Observers, ProgramSettings, XmlExporter, VideoCarrierDataFactory,
                VideoMetadataFactory, FFMPEGRunner, FFMPEGArgumentsFactory, FFProbeRunner, MediaInfoProvider,
                ImageProcessor, PlaceHolderFactory);

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
                            Path.Combine(ExpectedOriginalsDirectory, PreservationFileName), Arg.Any<CancellationToken>());

                        FileProvider.Received().MoveFileAsync(
                            Path.Combine(InputDirectory, NonNormalPresIntFileName),
                            Path.Combine(ExpectedOriginalsDirectory, PreservationIntermediateFileName), Arg.Any<CancellationToken>());

                        FileProvider.Received().MoveFileAsync(
                            Path.Combine(InputDirectory, NonNormalProductionFileName),
                            Path.Combine(ExpectedOriginalsDirectory, ProductionFileName), Arg.Any<CancellationToken>());
                    }
                }

                public class WhenFileNamesAreNormal : WhenMovingFilesToProcessingDirectory
                {
                    public class WhenPreservationIntermediateMasterPresent : WhenMovingFilesToProcessingDirectory
                    {
                        protected override void DoCustomSetup()
                        {
                            base.DoCustomSetup();

                            ModelList = new List<AbstractFile> { PresAbstractFile, PresIntAbstractFile };
                        }

                        [Test]
                        public void ItShouldMovePreservationIntermediateMasterToOriginalsDirectory()
                        {
                            FileProvider.Received()
                                .MoveFileAsync(Path.Combine(InputDirectory, PreservationIntermediateFileName),
                                    Path.Combine(ExpectedOriginalsDirectory, PreservationIntermediateFileName), Arg.Any<CancellationToken>());
                        }
                    }

                    public class WhenProductionMasterPresent : WhenMovingFilesToProcessingDirectory
                    {
                        protected override void DoCustomSetup()
                        {
                            base.DoCustomSetup();

                            ModelList = new List<AbstractFile> { PresAbstractFile, ProdAbstractFile };
                        }

                        [Test]
                        public void ItShouldMovePreservationIntermediateMasterToOriginalsDirectory()
                        {
                            FileProvider.Received().MoveFileAsync(Path.Combine(InputDirectory, ProductionFileName),
                                Path.Combine(ExpectedOriginalsDirectory, ProductionFileName), Arg.Any<CancellationToken>());
                        }
                    }

                    [Test]
                    public void ItShouldMovePresentationFileToOriginalsDirectory()
                    {
                        FileProvider.Received()
                            .MoveFileAsync(Path.Combine(InputDirectory, PreservationFileName),
                                Path.Combine(ExpectedOriginalsDirectory, PreservationFileName), Arg.Any<CancellationToken>());
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
                    MetadataProvider.Received().GetObjectMetadata<VideoPodMetadata>(Barcode, Arg.Any<CancellationToken>());
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
                        FFMPEGRunner.Received().Normalize(Arg.Is<AbstractFile>(m => m.Filename.Equals(model.Filename)),
                            Arg.Any<ArgumentBuilder>(), Arg.Any<CancellationToken>());
                    }
                }

                [Test]
                public void ItShouldVerifyAllNormalizedFiles()
                {
                    FFMPEGRunner.Received().Verify(Arg.Is<List<AbstractFile>>(l => l.SequenceEqual(ModelList)), Arg.Any<CancellationToken>());
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

                        ModelList = new List<AbstractFile> { PresAbstractFile, PresIntAbstractFile };
                        ExpectedMasterModel = PresIntAbstractFile;
                    }
                }

                [Test]
                public void ItShouldCallMetadataFactoryWithMezzanineMasterAsTarget()
                {
                    VideoMetadataFactory.Received().Generate(
                        Arg.Any<List<AbstractFile>>(),
                        Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()),
                        Arg.Any<VideoPodMetadata>());
                }

                [Test]
                public void ItShouldCreateAccessFileFromMezzanineMaster()
                {
                    FFMPEGRunner.Received().CreateAccessDerivative(
                        Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()), Arg.Any<ArgumentBuilder>(),
                        Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCreateProdFromMasterWithExpectedArgument()
                {
                    var expectedArguments = MezzArguments.AddArguments(ExpectedMetadata.AsArguments());

                    FFMPEGRunner.Received().CreateProdOrMezzDerivative(ExpectedMasterModel,
                        Arg.Is<AbstractFile>(m => m.IsMezzanineVersion()), Arg.Is<ArgumentBuilder>(v => v.SequenceEqual(expectedArguments)),
                        Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());
                }
            }

            public class WhenImportingImages : WhenNothingGoesWrong
            {
                public class WhenImagesAreImported : WhenImportingImages
                {
                    private AbstractFile Label { get; set; }
                    private List<AbstractFile> ReceivedModelList { get; set; }

                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();
                        Label = new TiffImageFile(new UnknownFile($"{ProjectCode}_{Barcode}_01_label.tif"));

                        ImageProcessor.ImportMediaImages(Arg.Any<string>(), Arg.Any<CancellationToken>())
                            .Returns(new List<AbstractFile> { Label });

                        VideoCarrierDataFactory.When(mg => mg.Generate(Arg.Any<VideoPodMetadata>(), Arg.Any<string>(), Arg.Any<List<AbstractFile>>(), Arg.Any<CancellationToken>()))
                       .Do(x => { ReceivedModelList = x.Arg<List<AbstractFile>>(); });
                    }

                    [Test]
                    public void ModelListShouldIncludeLabelModel()
                    {
                        Assert.That(ReceivedModelList.Count(m => m.Equals(Label)), Is.EqualTo(1));
                    }
                }

                [Test]
                public void ItShouldCallImageImporterCorrectly()
                {
                    ImageProcessor.Received().ImportMediaImages(Barcode, Arg.Is<CancellationToken>(ct => ct != null));
                }
            }

            public class WhenAddingPlaceHolders : WhenNothingGoesWrong
            {
                public class WhenNoPlaceHoldersAreNeeded : WhenAddingPlaceHolders
                {
                    public void ItShouldLogThatNoPlaceHoldersAreNeeded()
                    {
                        Observers.Received().Log("No place-holders to add");
                    }
                }

                public class WhenPlaceHoldersAreNeeded : WhenAddingPlaceHolders
                {

                    private List<AbstractFile> PlaceHolders = new List<AbstractFile>
                    {
                        new VideoPreservationFile(new PlaceHolderFile(ProjectCode, Barcode,3, ".wav")),
                        new MezzanineFile(new PlaceHolderFile(ProjectCode, Barcode, 3, ".wav")),
                        new AccessFile(new PlaceHolderFile(ProjectCode, Barcode, 3, ".wav"))
                    };

                    private List<AbstractFile> ReceivedModelList { get; set; }
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        PlaceHolderFactory.GetPlaceHoldersToAdd(Arg.Any<IMediaFormat>(), Arg.Any<List<AbstractFile>>())
                            .Returns(PlaceHolders);

                        VideoCarrierDataFactory.When(mg => mg.Generate(Arg.Any<VideoPodMetadata>(), Arg.Any<string>(), Arg.Any<List<AbstractFile>>(), Arg.Any<CancellationToken>()))
                       .Do(x => { ReceivedModelList = x.Arg<List<AbstractFile>>(); });
                    }

                    [Test]
                    public void ItShouldLogAllPlaceHolderEntries()
                    {
                        foreach (var fileModel in PlaceHolders)
                        {
                            Observers.Received().Log("Adding placeholder: {0}", fileModel.Filename);
                        }
                    }

                    [Test]
                    public void ModelListShouldIncludeAddedPlaceHolderModels()
                    {
                        foreach (var fileModel in PlaceHolders)
                        {
                            Assert.That(ReceivedModelList.Contains(fileModel), Is.True,
                                $"Model list should include place-holder {fileModel.Filename}");
                        }
                    }

                    [Test]
                    public void ItShouldNotHashPlaceHolderFiles()
                    {
                        foreach (var fileModel in PlaceHolders)
                        {
                            Hasher.DidNotReceive().Hash(fileModel, Arg.Any<CancellationToken>());
                        }
                    }

                    [Test]
                    public void ItShouldNotAttemptToCopyPlaceHolderFiles()
                    {
                        foreach (var fileModel in PlaceHolders)
                        {
                            FileProvider.DidNotReceive().CopyFileAsync(
                                Path.Combine(ExpectedProcessingDirectory, fileModel.Filename),
                                Arg.Any<string>(),
                                Arg.Any<CancellationToken>());
                        }
                    }
                }

                [Test]
                public void ItShouldCallPlaceHolderGenerator()
                {
                    PlaceHolderFactory.Received().GetPlaceHoldersToAdd(Arg.Any<IMediaFormat>(), Arg.Any<List<AbstractFile>>());
                }

                [Test]
                public void ItShouldWriteSectionHeader()
                {
                    Observers.Received().BeginSection("Adding Placeholder Entries");
                }

            }

            public class WhenGeneratingXmlManifest : WhenNothingGoesWrong
            {
                private VideoCarrier VideoCarrier { get; set; }

                private List<AbstractFile> ReceivedModelList { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    VideoCarrier = new VideoCarrier();
                    VideoCarrierDataFactory.When(
                        mg => mg.Generate(Arg.Any<VideoPodMetadata>(), Arg.Any<string>(), Arg.Any<List<AbstractFile>>(), Arg.Any<CancellationToken>()))
                        .Do(x => { ReceivedModelList = x.Arg<List<AbstractFile>>(); });
                    VideoCarrierDataFactory.Generate(Arg.Any<VideoPodMetadata>(), Arg.Any<string>(), Arg.Any<List<AbstractFile>>(), Arg.Any<CancellationToken>())
                        .Returns(VideoCarrier);
                }

                public class WhenPreservationIntermediateModelPresent : WhenGeneratingXmlManifest
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFile> { PresAbstractFile, PresIntAbstractFile };
                    }

                    [Test]
                    public void ItShouldPassSinglePresentationIntermediateModelToGenerator()
                    {
                        VideoCarrierDataFactory.Received().Generate(
                            Arg.Any<VideoPodMetadata>(),
                            Arg.Any<string>(),
                            Arg.Is<List<AbstractFile>>(
                                l => l.SingleOrDefault(m => m.IsPreservationIntermediateVersion()) != null), Arg.Any<CancellationToken>());
                    }
                }

                private IEnumerable<AbstractFile> GetMasters()
                {
                    return ModelList.Where(m =>
                        m is AbstractPreservationFile ||
                        m is AbstractPreservationIntermediateFile).ToList();
                }

                private T GetDerivativeForMaster<T>(AbstractFile master) where T : AbstractFile
                {
                    return ReceivedModelList.SingleOrDefault(m =>
                        m is T &&
                        m.ProjectCode.Equals(master.ProjectCode) &&
                        m.BarCode.Equals(master.BarCode) &&
                        m.SequenceIndicator.Equals(master.SequenceIndicator)) as T;
                }

                private QualityControlFile GetQualityControlFileForMaster(AbstractFile master)
                {
                    return ReceivedModelList.SingleOrDefault(m =>
                        m is QualityControlFile &&
                        m.ProjectCode.Equals(master.ProjectCode) &&
                        m.BarCode.Equals(master.BarCode) &&
                        m.FileUsage.GetType() == master.FileUsage.GetType() &&
                        m.SequenceIndicator.Equals(master.SequenceIndicator)) as QualityControlFile;
                }

                [Test]
                public void ItShouldCallExportToFileCorrectly()
                {
                    XmlExporter.Received().ExportToFile(Arg.Is<ExportableManifest>(iu => iu.Carrier.Equals(VideoCarrier)),
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
                public void ItShouldPassExpectedAccessModelsToGenerator()
                {
                    var mastersCount = GetMasters().Count();
                    var derivativeCount = Enumerable.Count(GetMasters().Select(GetDerivativeForMaster<AccessFile>));
                    Assert.That(derivativeCount, Is.EqualTo(mastersCount));
                }

                [Test]
                public void ItShouldPassExpectedMastersToGenerator()
                {
                    foreach (var master in GetMasters())
                    {
                        Assert.That(ReceivedModelList.Contains(master));
                    }
                }

                [Test]
                public void ItShouldPassExpectedMezzModelsToGenerator()
                {
                    var mastersCount = GetMasters().Count();
                    var derivativeCount = Enumerable.Count(GetMasters().Select(GetDerivativeForMaster<MezzanineFile>));
                    Assert.That(derivativeCount, Is.EqualTo(mastersCount));
                }

                [Test]
                public void ItShouldPassSinglePreservationModelToGenerator()
                {
                    VideoCarrierDataFactory.Received().Generate(
                        Arg.Any<VideoPodMetadata>(),
                        Arg.Any<string>(),
                        Arg.Is<List<AbstractFile>>(l => l.SingleOrDefault(m => m.IsPreservationVersion()) != null), Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldPassSingleQualityControlModelToGeneratorForEveryPresOrPresIntMaster()
                {
                    var mastersCount = GetMasters().Count();
                    var qualityControlFileCount = GetMasters().Select(GetQualityControlFileForMaster).Count();
                    Assert.That(qualityControlFileCount, Is.EqualTo(mastersCount));
                }

                [Test]
                public void ItShouldPassCorrectDigitizingEntityToFactory()
                {
                    VideoCarrierDataFactory.Received()
                        .Generate(Arg.Any<VideoPodMetadata>(), DigitizingEntity, Arg.Any<List<AbstractFile>>(), Arg.Any<CancellationToken>());
                }
            }

            public class WhenHashingFiles : WhenNothingGoesWrong
            {
                public override async Task BeforeEach()
                {
                    await base.BeforeEach();

                    ProcessedModelList = VideoCarrierDataFactory.ReceivedCalls()
                        .First().GetArguments()[2] as List<AbstractFile>;
                }

                private List<AbstractFile> ProcessedModelList { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    Hasher.Hash(Arg.Any<AbstractFile>(), Arg.Any<CancellationToken>())
                        .Returns(x => Task.FromResult($"{x.Arg<AbstractFile>().Filename} checksum"));

                    Hasher.Hash(Arg.Any<string>(), Arg.Any<CancellationToken>())
                        .Returns(x => Task.FromResult($"{Path.GetFileName(x.Arg<string>())} checksum"));
                }

                [TestCase]
                public void ItShouldLogChecksumsCorrectly()
                {
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileName(PreservationFileName),
                            $"{PreservationFileName} checksum");
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileName(ProductionFileName),
                            $"{ProductionFileName} checksum");
                    Observers.Received()
                        .Log("{0} checksum: {1}", Path.GetFileName(AccessFileName),
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
                    Assert.That(
                        ProcessedModelList.Single(m => m.IsSameAs(new UnknownFile(PreservationFileName))).Checksum,
                        Is.EqualTo($"{PreservationFileName} checksum"));
                }

                [Test]
                public void ItShouldAssignHashValueCorrectlytoProductionVersion()
                {
                    Assert.That(
                        ProcessedModelList.Single(m => m.IsSameAs(new UnknownFile(ProductionFileName))).Checksum,
                        Is.EqualTo($"{ProductionFileName} checksum"));
                }

                [Test]
                public void ItShouldCallHasherForAccessVersion()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(AccessFileName))), Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCallHasherForPreservationMaster()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(PreservationFileName))), Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCallHasherForProductionVersion()
                {
                    Hasher.Received().Hash(Arg.Is<AbstractFile>(m => m.IsSameAs(new UnknownFile(ProductionFileName))), Arg.Any<CancellationToken>());
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
                    ExpectedFiles = 5; // prod master, pres master, access, xml manifest, qc file
                }

                public class WhenPreservationIntermediateModelPresent : WhenCopyingFilesToDropBoxFolder
                {
                    protected override void DoCustomSetup()
                    {
                        base.DoCustomSetup();

                        ModelList = new List<AbstractFile> { PresAbstractFile, PresIntAbstractFile };
                        ExpectedFiles = 7;
                        // prod master, pres master, presInt master, access, xml manifest, pres qc file, pres int qc file
                    }

                    [Test]
                    public void ItShouldCopyPreservationIntermediateMasterToDropbox()
                    {
                        var sourcePath = Path.Combine(ExpectedProcessingDirectory, PreservationIntermediateFileName);
                        var targetPath = Path.Combine(ExpectedDropboxDirectory, PreservationIntermediateFileName);
                        FileProvider.Received().CopyFileAsync(sourcePath, targetPath, Arg.Any<CancellationToken>());
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
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath, Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCopyCorrectNumberOfFilesToDropBox()
                {
                    FileProvider.Received(ExpectedFiles).CopyFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCopyPreservationMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, PreservationFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, PreservationFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath, Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCopyProductionMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, ProductionFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, ProductionFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath, Arg.Any<CancellationToken>());
                }

                [Test]
                public void ItShouldCopyXmlManifestToDropbox()
                {
                    var sourcePath = Path.Combine(ExpectedProcessingDirectory, XmlManifestFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, XmlManifestFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath, Arg.Any<CancellationToken>());
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
                Assert.That(Result.Succeeded, Is.EqualTo(true));
                Assert.That(Result.Skipped, Is.EqualTo(false));
                Assert.That(Result.Failed, Is.EqualTo(false));
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
                    Assert.That(Result.Succeeded, Is.EqualTo(false));
                    Assert.That(Result.Skipped, Is.EqualTo(false));
                    Assert.That(Result.Failed, Is.EqualTo(true));
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
                    FileProvider.CopyFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
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