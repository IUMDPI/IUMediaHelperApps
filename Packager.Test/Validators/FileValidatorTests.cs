using NSubstitute;
using NUnit.Framework;
using Packager.Providers;
using Packager.Validators;

namespace Packager.Test.Validators
{
    [TestFixture]
    public class FileValidatorTests
    {
        private const string TestPath = "test path";
        private const string Name = "Test name";
        private IFileProvider Provider { get; set; }
        private bool ProviderResult { get; set; }
        private ValidationResult Result { get; set; }

        protected virtual void DoCustomSetup()
        {
            ProviderResult = true;
        }

        [SetUp]
        public void BeforeEach()
        {
            Provider = Substitute.For<IFileProvider>();
            Provider.FileExists(TestPath).Returns(ProviderResult);

            DoCustomSetup();

            var validator = new FileExistsValidator(Provider);
            Result = validator.Validate(TestPath, Name);
        }

        [Test]
        public void ItShouldCallProviderCorrectly()
        {
            Provider.Received().FileExists(TestPath);
        }

        [Test]
        public void ItShouldReturnCorrectResult()
        {
            Assert.That(Result.Result, Is.EqualTo(ProviderResult));
        }

        public class WhenThingsGoWrong : FileValidatorTests
        {
            protected override void DoCustomSetup()
            {
                ProviderResult = false;
            }

            [Test]
            public void ItShouldSetIssueCorrectly()
            {
                Assert.That(Result.Issue, Is.EqualTo(string.Format("Invalid path specified for {0}: {1}", Name, TestPath)));
            }
        }
    }
}