using System.Text;
using NUnit.Framework;
using Packager.Verifiers;

namespace Packager.Test.Verifiers
{
    [TestFixture]
    public class BwfMetaEditResultsVerifierTests
    {
        private const string FileName = "MDPI_11111111111111_01_pres.wav";
        private const string ModifiedLine = "MDPI_11111111111111_01_pres.wav: Is modified";
        private const string InvalidWavLine = "invalid wave: issue";
        private const string CanceledLine = "canceled";
        private const string ErrorDuringReadingLine = "error during reading";
        private const string ErrorDuringWritingLine = "error during writing";


        private static string BuildOutput(params string[] lines)
        {
            var builder = new StringBuilder();
            foreach (var line in lines)
            {
                builder.AppendLine(line);
            }

            return builder.ToString().ToLowerInvariant();
        }

        [TestCase(InvalidWavLine)]
        [TestCase(CanceledLine)]
        [TestCase(ErrorDuringReadingLine)]
        [TestCase(ErrorDuringWritingLine)]
        public void ShouldReturnFalseIfErrorLinePresent(string errorLine)
        {
            var verifier = new BwfMetaEditResultsVerifier();
            var output = BuildOutput("testing testing", ModifiedLine, errorLine);
            Assert.That(verifier.Verify(output, FileName), Is.False);
        }

        [Test]
        public void ShouldReturnFalseIfIsModifiedLineIsNotPresent()
        {
            var verifier = new BwfMetaEditResultsVerifier();
            var output = BuildOutput("testing testing");
            Assert.That(verifier.Verify(output, FileName), Is.False);
        }


        [Test]
        public void ShouldReturnTrueIfIsModifiedLineIsPresentWithoutErrorLines()
        {
            var verifier = new BwfMetaEditResultsVerifier();
            var output = BuildOutput("testing testing", ModifiedLine);
            Assert.That(verifier.Verify(output, FileName), Is.True);
        }
    }
}