using Common.Models;
using NUnit.Framework;

namespace Common.Tests.Models
{
    [TestFixture]
    public class FileUsagesTests
    {
        private static object[] _usagesCorrectCases =
        {
            new object[] {FileUsages.PreservationMaster, "pres", "Preservation Master", 0},
            new object[] {FileUsages.PreservationToneReference, "presRef", "Preservation Master Tone Reference File", 1},
            new object[] {FileUsages.PreservationIntermediateMaster, "presInt", "Preservation Master - Intermediate", 2},
            new object[] {FileUsages.PreservationIntermediateToneReference, "intRef", "Preservation Master - Intermediate Tone Reference File", 3},
            new object[] {FileUsages.ProductionMaster, "prod", "Production Master", 4},
            new object[] {FileUsages.MezzanineFile, "mezz", "Mezzanine File", 5},
            new object[] {FileUsages.AccessFile, "access", "Access File", 6},
            new object[] {FileUsages.LabelImageFile, "label", "Label Image File", 7},
            new object[] {new QualityControlFileUsage(FileUsages.PreservationMaster), FileUsages.PreservationMaster.FileUse, FileUsages.PreservationMaster.FullFileUse, 8},
            new object[] {new UnknownFileUsage("", "Raw object file"), "", "Raw object file", 100},
            new object[] {FileUsages.XmlFile, "", "Xml File", 100},
        };

        private static object[] _getUsageCases =
        {
            new object[] {"pres",FileUsages.PreservationMaster},
            new object[] {"presInt",FileUsages.PreservationIntermediateMaster},
            new object[] {"prod", FileUsages.ProductionMaster},
            new object[] {"mezz", FileUsages.MezzanineFile},
            new object[] {"access", FileUsages.AccessFile},
            new object[] {"label", FileUsages.LabelImageFile },
            new object[] {"presRef", FileUsages.PreservationToneReference},
            new object[] {"intRef", FileUsages.PreservationIntermediateToneReference},
            new object[] {string.Empty, new UnknownFileUsage(string.Empty, "Raw object file")},
            new object[] {null, new UnknownFileUsage(null, "Raw object file")},
            new object[] {"some other value", new UnknownFileUsage("some other value", "Raw object file") }
        };

        [Test, TestCaseSource(nameof(_usagesCorrectCases))]
        public void UsagesShouldBeCorrect(IFileUsage usage, string expectedFileUse, string expectedFullFileUse, int expectedPrecedence)
        {
            Assert.That(usage.FileUse, Is.EqualTo(expectedFileUse),
                $"File use should be {expectedFileUse}");
            Assert.That(usage.FullFileUse, Is.EqualTo(expectedFullFileUse),
                $"Full file use should be {expectedFullFileUse}");
            Assert.That(usage.Precedence, Is.EqualTo(expectedPrecedence),
                $"Precendence should be {expectedPrecedence}");
        }

        [Test, TestCaseSource(nameof(_getUsageCases))]
        public void GetUsageShouldReturnCorrectResult(string fileUse, IFileUsage expected)
        {
            var result = FileUsages.GetUsage(fileUse);
            var description = $"{fileUse} should produce {expected.FullFileUse} file usage object";

            if (expected is UnknownFileUsage)
            {
                Assert.That(result is UnknownFileUsage, description);
                Assert.That(result.FileUse, Is.EqualTo(fileUse));
                Assert.That(result.FullFileUse, Is.EqualTo("Raw object file"));
            }
            else
            {
                Assert.That(result, Is.EqualTo(expected), description);
            }

        }
    }
}
