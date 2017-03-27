using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using NUnit.Framework;

namespace Common.Tests.Models
{
    [TestFixture]
    public class FileUsagesTests
    {
        private static object[] _usagesCorrectCases =
        {
            new object[] {FileUsages.PreservationMaster, "pres", "Preservation Master"},
            new object[] {FileUsages.PreservationIntermediateMaster, "presInt", "Preservation Master - Intermediate"},
            new object[] {FileUsages.ProductionMaster, "prod", "Production Master"},
            new object[] {FileUsages.MezzanineFile, "mezz", "Mezzanine File"},
            new object[] {FileUsages.AccessFile, "access", "Access File"},
            new object[] {FileUsages.LabelImageFile, "label", "Label Image File"}
        };

        private static object[] _getUsageCases =
        {
            new object[] {"pres",FileUsages.PreservationMaster},
            new object[] {"presInt",FileUsages.PreservationIntermediateMaster},
            new object[] {"prod", FileUsages.ProductionMaster},
            new object[] {"mezz", FileUsages.MezzanineFile},
            new object[] {"access", FileUsages.AccessFile},
            new object[] { "label", FileUsages.LabelImageFile }
        };

        [Test, TestCaseSource(nameof(_usagesCorrectCases))]
        public void UsagesShouldBeCorrect(FileUsages usage, string expectedFileUse, string expectedFullFileUse)
        {
            Assert.That(usage.FileUse, Is.EqualTo(expectedFileUse), 
                $"File use should be {expectedFileUse}");
            Assert.That(usage.FullFileUse, Is.EqualTo(expectedFullFileUse),
                $"Full file use should be {expectedFullFileUse}");
        }

        [Test, TestCaseSource(nameof(_getUsageCases))]
        public void GetUsageShouldReturnCorrectResult(string fileUse, FileUsages expected)
        {
            var result = FileUsages.GetUsage(fileUse);
            Assert.That(result, Is.EqualTo(expected), $"{fileUse} should produce {expected.FullFileUse} file usage object");
        } 
    }
}
