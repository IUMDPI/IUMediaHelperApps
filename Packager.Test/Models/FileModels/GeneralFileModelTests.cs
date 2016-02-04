using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class GeneralFileModelTests
    {
        [TestCase("lowercase_barcode_01_pres.wav", "LOWERCASE")]
        [TestCase("mixedCase_barcode_01_pres.wav", "MIXEDCASE")]
        [TestCase("UPPERCASE_barcode_01_pres.wav", "UPPERCASE")]
        public void ProjectCodeShouldAlwaysBeUpperCase(string value, string expected)
        {
            var model = new UnknownFile(value);
            Assert.That(model.ProjectCode, Is.EqualTo(expected.ToUpperInvariant()));
        }

        [TestCase("mdpi_4890764553278906_01_pres.wav", true)]
        [TestCase("MDPI_4890764553278906_1_pres.wav", true)]
        [TestCase("mdpi_4890764553278906_02_pres.wav", false)]
        [TestCase("MDPI_4890764553278906_2_pres.wav", false)]
        [TestCase("mdpi_4890764553278906_02_prod.wav", false)]
        [TestCase("MDPI_4890764553278906_2_mezz.wav", false)]
        public void IsSameAsShouldWorkCorrectly(string filename, bool expected)
        {
            var original = new AudioPreservationFile(
                new UnknownFile("mdpi_4890764553278906_01_pres.wav"));
            
            Assert.That(original.IsSameAs(new UnknownFile(filename)), Is.EqualTo(expected));
        }
    }
}