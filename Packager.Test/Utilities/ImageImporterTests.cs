using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.SettingsModels;
using Packager.Providers;
using Packager.Utilities.Hashing;
using Packager.Utilities.Images;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class LabelImageImporterTests
    {
        private const string ProjectCode = "MDPI";
        private const string ImageDirectory = "ImageDirectory";
        private const string ProcessingDirectory = "ProcessingDirectory";
        private const string Barcode = "11111111111";

        private IProgramSettings Settings { get; set; }
        private IFileProvider FileProvider { get; set; }
        private IDirectoryProvider DirectoryProvider { get; set; }
        private IHasher Hasher { get; set; }
        private ILabelImageImporter ImageImporter { get; set; }

        private List<string> SourceFiles { get; set; }
        private static string ValidFilename1 => $"{ProjectCode}_{Barcode}_01_label.tif";
        private static string ValidFilename2 => $"{ProjectCode}_{Barcode}_02_label.tif";
        private static string InvalidFilename => $"{ProjectCode}_{Barcode}_01_invalid.tif";

        private static string ExpectedSourceFolder => Path.Combine(ImageDirectory, $"{ProjectCode}_{Barcode}");

        [SetUp]
        public void BeforeEach()
        {
            SourceFiles = new List<string> { ValidFilename1, ValidFilename2, InvalidFilename };

            Settings = Substitute.For<IProgramSettings>();
            Settings.ProjectCode.Returns(ProjectCode);
            Settings.ImageDirectory.Returns(ImageDirectory);
            Settings.ProcessingDirectory.Returns(ProcessingDirectory);

            FileProvider = Substitute.For<IFileProvider>();
            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            DirectoryProvider.DirectoryExists(ExpectedSourceFolder).Returns(true);
            DirectoryProvider.EnumerateFiles(ExpectedSourceFolder).Returns(SourceFiles);

            Hasher = Substitute.For<IHasher>();
            Hasher.Hash(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(args => $"{Path.GetFileName(args.Arg<string>())} hash");

            ImageImporter = new LabelImageImporter(Settings, FileProvider, DirectoryProvider, Hasher);
        }

        private string GetSourcePath(string filename)
        {
            return Path.Combine(ExpectedSourceFolder, filename);
        }

        private string GetDestPath(string filename)
        {
            return Path.Combine(ProcessingDirectory, $"{ProjectCode}_{Barcode}", filename);
        }

        [Test]
        public async void ImporterShouldCallDirectoryProviderExistsCorrectly()
        {
            await ImageImporter.ImportMediaImages(Barcode, new CancellationToken());
            DirectoryProvider.Received().DirectoryExists(ExpectedSourceFolder);
        }

        [Test]
        public async void ImporterShouldReturnEmptyListIfFolderMissing()
        {
            DirectoryProvider.DirectoryExists(ExpectedSourceFolder).Returns(false);
            var result = await ImageImporter.ImportMediaImages(Barcode, new CancellationToken());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async void ImporterShouldCallDirectoryProviderEnumerateFilesCorrectly()
        {
            await ImageImporter.ImportMediaImages(Barcode, new CancellationToken());
            DirectoryProvider.Received().EnumerateFiles(ExpectedSourceFolder);
        }

        [Test]
        public async void ImporterShouldReturnEmptyListIfNoFilesPresent()
        {
            DirectoryProvider.EnumerateFiles(ExpectedSourceFolder).Returns(new List<string>());
            var result = await ImageImporter.ImportMediaImages(Barcode, new CancellationToken());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ImporterShouldHashAllEligibleSourceFiles()
        {
            ImageImporter.ImportMediaImages(Barcode, new CancellationToken()).Wait();

            Hasher.Received(1).Hash(GetSourcePath(ValidFilename1), new CancellationToken());
            Hasher.Received(1).Hash(GetSourcePath(ValidFilename2), new CancellationToken());
            Hasher.Received(0).Hash(GetSourcePath(InvalidFilename), new CancellationToken());

        }

        [Test]
        public void ImporterShouldCopyAllEligibleSourceFiles()
        {
            ImageImporter.ImportMediaImages(Barcode, new CancellationToken()).Wait();

            FileProvider.Received(1).CopyFileAsync(
                GetSourcePath(ValidFilename1),
                GetDestPath(ValidFilename1),
                new CancellationToken());

            FileProvider.Received(1).CopyFileAsync(
              GetSourcePath(ValidFilename2),
              GetDestPath(ValidFilename2),
              new CancellationToken());

            FileProvider.Received(0).CopyFileAsync(
              GetSourcePath(InvalidFilename),
              GetDestPath(InvalidFilename),
              new CancellationToken());

        }

        [Test]
        public void ImporterShouldHashAllEligibleDestinationFiles()
        {
            ImageImporter.ImportMediaImages(Barcode, new CancellationToken()).Wait();

            Hasher.Received(1).Hash(GetDestPath(ValidFilename1), new CancellationToken());
            Hasher.Received(1).Hash(GetDestPath(ValidFilename2), new CancellationToken());
            Hasher.Received(0).Hash(GetDestPath(InvalidFilename), new CancellationToken());
        }

        [Test]
        public void ImporterShouldThrowExceptionIfSourceAndDestHashesNotEqual()
        {
            var hash1 = "hash1";
            var hash2 = "hash2";
            Hasher.Hash(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(hash1, hash2);

            var exception = Assert.Throws<Exception>(async () => await ImageImporter.ImportMediaImages(Barcode, new CancellationToken()));
            Assert.That(exception.Message, Is.EqualTo($"copy hash ({hash2}) is not equal to original hash ({hash1})."));

        }

        [Test]
        public async void ImporterShouldReturnCorrectResultsIfNoIssues()
        {
            var result = await ImageImporter.ImportMediaImages(Barcode, new CancellationToken());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(f => f.Filename.Equals(ValidFilename1)), Is.EqualTo(1));
            Assert.That(result.Count(f => f.Filename.Equals(ValidFilename2)), Is.EqualTo(1));
        }
    }
}
