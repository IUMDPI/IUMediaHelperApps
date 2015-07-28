using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Processors;

namespace Packager.Test.Processors
{
    [TestFixture]
    public class AudioProcessorTests : AbstractProcessorTests
    {
        private Guid SectionGuid { get; set; }

        private const string ProdCommandLineArgs = "-c:a pcm_s24le -b:a 128k -strict -2 -ar 96000";
        private const string AccessCommandLineArgs = "-c:a aac -b:a 128k -strict -2 -ar 48000";
        private const string FFMPEGPath = "ffmpeg.exe";

        protected override void DoCustomSetup()
        {
            SectionGuid = Guid.NewGuid();
            Observers.BeginSection("Processing Object: {0}", Barcode).Returns(SectionGuid);

            ProductionFileName = string.Format("{0}_{1}_01_prod.wav", ProjectCode, Barcode);
            PreservationFileName = string.Format("{0}_{1}_01_pres.wav", ProjectCode, Barcode);
            AccessFileName = string.Format("{0}_{1}_01_access.mp4", ProjectCode, Barcode);
            XmlManifestFileName = string.Format("{0}_{1}.xml", ProjectCode, Barcode);

            PresObjectFileModel = new ObjectFileModel(PreservationFileName);
            ProdObjectFileModel = new ObjectFileModel(ProductionFileName);
            AccessObjectFileModel = new ObjectFileModel(AccessFileName);

            Grouping = new List<AbstractFileModel> { PresObjectFileModel, ProdObjectFileModel }
                .GroupBy(m => m.BarCode).First();

            ProcessingDirectory = string.Format("{0}_{1}", ProjectCode, Barcode);

            DependencyProvider.MetadataProvider.Get(Barcode).Returns(Task.FromResult(new ConsolidatedPodMetadata { Success = true }));

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

            public class WhenEmbeddingMetadata : WhenNothingGoesWrong
            {
                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    Grouping = new List<AbstractFileModel> { PresObjectFileModel, ProdObjectFileModel, AccessObjectFileModel }
                        .GroupBy(m => m.BarCode).First();
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
                public void ItShouldPassTwoFileObjectsToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Count() == 2),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleProductionModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSinglePresentationModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldNotPassAccessModelToBextProcessor()
                {
                    BextProcessor.Received().EmbedBextMetadata(
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Any(m => m.IsAccessVersion()) == false),
                        Arg.Any<ConsolidatedPodMetadata>(),
                        ProcessingDirectory);
                }
            }

            public class WhenGeneratingXmlManifest : WhenNothingGoesWrong
            {
                private CarrierData CarrierData { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    CarrierData = new CarrierData();
                    MetadataGenerator.GenerateMetadata(null, null, "").ReturnsForAnyArgs(CarrierData);
                }

                [Test]
                public void ItShouldPassThreeFileObjectsToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                       Arg.Any<ConsolidatedPodMetadata>(),
                       Arg.Is<IEnumerable<ObjectFileModel>>(l => l.Count()==3),
                       ProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleProductionModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsProductionVersion()) != null),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSinglePreservationModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsPreservationVersion()) != null),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldPassSingleAccessModelToGenerator()
                {
                    MetadataGenerator.Received().GenerateMetadata(
                        Arg.Any<ConsolidatedPodMetadata>(),
                        Arg.Is<IEnumerable<ObjectFileModel>>(l => l.SingleOrDefault(m => m.IsAccessVersion()) != null),
                        ProcessingDirectory);
                }

                [Test]
                public void ItShouldCallExportToFileCorrectly()
                {
                    XmlExporter.Received().ExportToFile(Arg.Is<IU>(iu => iu.Carrier.Equals(CarrierData)),
                        Path.Combine(ProcessingDirectory, XmlManifestFileName));
                }
            }

            public class WhenCopyingFilesToDropBoxFolder : WhenNothingGoesWrong
            {
                private string ExpectedDropboxDirectory { get; set; }

                protected override void DoCustomSetup()
                {
                    base.DoCustomSetup();

                    ExpectedDropboxDirectory = Path.Combine(DropBoxRoot, string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), Barcode));
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
                public void ItShouldCopyFourFilesToDropBox()
                {
                    FileProvider.Received(4).CopyFileAsync(Arg.Any<string>(), Arg.Any<string>());
                }

                [Test]
                public void ItShouldCopyPreservationMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ProcessingDirectory, PreservationFileName);
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
                    var sourcePath = Path.Combine(ProcessingDirectory, ProductionFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, ProductionFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCopyAccessMasterToDropbox()
                {
                    var sourcePath = Path.Combine(ProcessingDirectory, AccessFileName);
                    var targetPath = Path.Combine(ExpectedDropboxDirectory, AccessFileName);
                    FileProvider.Received().CopyFileAsync(sourcePath, targetPath);
                }

                [Test]
                public void ItShouldCopyXmlManifestToDropbox()
                {
                    var sourcePath = Path.Combine(ProcessingDirectory, XmlManifestFileName);
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


    }
}