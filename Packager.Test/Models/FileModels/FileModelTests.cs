﻿using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModelTests
{
    [TestFixture]
    public abstract class AbstractObjectFileModelTests
    {
        public string FileName { get; set; }

        private string ExpectedFileUse { get; set; }
        private string ExpectedFullFileUse { get; set; }
        private string ExpectedExtension { get; set; }
        private string ExpectedToFileName { get; set; }

        private string ExpectedSameFileName { get; set; }

        private readonly UnknownFile _originalFile = new UnknownFile("mdpi_4890764553278906_01_pres.wav");

        private AbstractFile Instance { get; set; }

        [TestCase("mdpi", true)]
        [TestCase("mdPi", true)]
        [TestCase("MDPI", true)]
        [TestCase("notmdpi", false)]
        [TestCase("NOTMDPI", false)]
        [TestCase("NotMDPI", false)]
        public void BelongsToProjectShouldWorkAsExpected(string projectCode, bool expected)
        {
            Assert.That(Instance.BelongsToProject(projectCode), Is.EqualTo(expected));
        }

        private object ExpectedFrameMd5FileName { get; set; }

        public class AudioPreservationFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "pres";
                ExpectedFullFileUse = "Preservation Master";
                ExpectedExtension = ".wav";
                ExpectedToFileName = "MDPI_4890764553278906_01_pres.wav";
                ExpectedSameFileName = "mdpi_4890764553278906_1_pres.wav";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_pres.wav.framemd5";
                Instance = new AudioPreservationFile(_originalFile);
            }
        }

        public class VideoPreservationFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "pres";
                ExpectedFullFileUse = "Preservation Master";
                ExpectedExtension = ".mkv";
                ExpectedToFileName = "MDPI_4890764553278906_01_pres.mkv";
                ExpectedSameFileName = "mdpi_4890764553278906_1_pres.mkv";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_pres.mkv.framemd5";
                Instance = new VideoPreservationFile(_originalFile);
            }
        }

        public class AudioPreservationIntermediateFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "presInt";
                ExpectedFullFileUse = "Preservation Master - Intermediate";
                ExpectedExtension = ".wav";
                ExpectedToFileName = "MDPI_4890764553278906_01_presInt.wav";
                ExpectedSameFileName = "mdpi_4890764553278906_1_presInt.wav";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_presInt.wav.framemd5";
                Instance = new AudioPreservationIntermediateFile(_originalFile);
            }
        }

        public class VideoPreservationIntermediateFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "presInt";
                ExpectedFullFileUse = "Preservation Master - Intermediate";
                ExpectedExtension = ".mkv";
                ExpectedToFileName = "MDPI_4890764553278906_01_presInt.mkv";
                ExpectedSameFileName = "mdpi_4890764553278906_1_presInt.mkv";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_presInt.mkv.framemd5";
                Instance = new VideoPreservationIntermediateFile(_originalFile);
            }
        }

        public class ProductionFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "prod";
                ExpectedFullFileUse = "Production Master";
                ExpectedExtension = ".wav";
                ExpectedToFileName = "MDPI_4890764553278906_01_prod.wav";
                ExpectedSameFileName = "mdpi_4890764553278906_1_prod.wav";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_prod.wav.framemd5";
                Instance = new ProductionFile(_originalFile);
            }
        }

        public class MezzanineFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "mezz";
                ExpectedFullFileUse = "Mezzanine File Version";
                ExpectedExtension = ".mov";
                ExpectedToFileName = "MDPI_4890764553278906_01_mezz.mov";
                ExpectedSameFileName = "mdpi_4890764553278906_1_mezz.mov";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_mezz.mov.framemd5";
                Instance = new MezzanineFile(_originalFile);
            }
        }

        public class AccessFileModelTests : AbstractObjectFileModelTests
        {
            [SetUp]
            public void BeforeEach()
            {
                ExpectedFileUse = "access";
                ExpectedFullFileUse = "Access File Version";
                ExpectedExtension = ".mp4";
                ExpectedToFileName = "MDPI_4890764553278906_01_access.mp4";
                ExpectedSameFileName = "mdpi_4890764553278906_1_access.mp4";
                ExpectedFrameMd5FileName = "MDPI_4890764553278906_01_access.mp4.framemd5";
                Instance = new AccessFile(_originalFile);
            }
        }

        [Test]
        public void BarCodeShouldBeCorrect()
        {
            Assert.That(Instance.BarCode, Is.EqualTo("4890764553278906"));
        }

        [Test]
        public void ExtensionShouldBeCorrect()
        {
            Assert.That(Instance.Extension, Is.EqualTo(ExpectedExtension));
        }

        [Test]
        public void FileUseShouldBeCorrect()
        {
            Assert.That(Instance.FileUse, Is.EqualTo(ExpectedFileUse));
        }

        [Test]
        public void FullFileUseShouldBeCorrect()
        {
            Assert.That(Instance.FullFileUse, Is.EqualTo(ExpectedFullFileUse));
        }

        [Test]
        public void GetFolderNameShouldBeCorrect()
        {
            Assert.That(Instance.GetFolderName(), Is.EqualTo("MDPI_4890764553278906"));
        }

        [Test]
        public void IsSameAsShouldWorkCorrectlyForSameFileName()
        {
            var compare = new UnknownFile(ExpectedSameFileName);
            Assert.That(Instance.IsSameAs(compare), Is.True);
        }

        [Test]
        public void IsValidShouldReturnTrue()
        {
            Assert.That(Instance.IsValid(), Is.True);
        }

        [Test]
        public void ProjectCodeShouldBeCorrect()
        {
            Assert.That(Instance.ProjectCode, Is.EqualTo("MDPI"));
        }

        [Test]
        public void SequenceIndicatorShouldBeCorrect()
        {
            Assert.That(Instance.SequenceIndicator, Is.EqualTo(1));
        }

        [Test]
        public void ToFileNameShouldBeCorrect()
        {
            Assert.That(Instance.Filename, Is.EqualTo(ExpectedToFileName));
        }

        [Test]
        public void ToFrameMd5FilenameShouldReturnCorrectResult()
        {
            Assert.That(Instance.ToFrameMd5Filename(), Is.EqualTo(ExpectedFrameMd5FileName));
        }
    }
}