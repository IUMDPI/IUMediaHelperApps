using NUnit.Framework;
using Packager.Validators;

namespace Packager.Test.Validators
{
    [TestFixture]
    public class UriValidatorTests
    {
        private const string Name = "Test name";
        private string TestValue { get; set; }
        private bool ExpectedResult { get; set; }

        private ValidationResult Result { get; set; }

        protected virtual void DoCustomSetup()
        {
            TestValue = "http://www.test.org";
            ExpectedResult = true;
        }

        [SetUp]
        public void BeforeEach()
        {
            DoCustomSetup();

            var validator = new UriValidator();
            Result = validator.Validate(TestValue, Name);
        }

        [Test]
        public void ItShouldReturnCorrectResult()
        {
            Assert.That(Result.Result, Is.EqualTo(ExpectedResult));
        }

        public class WhenUriInvalid : UriValidatorTests
        {
            protected override void DoCustomSetup()
            {
                TestValue = "test.org";
                ExpectedResult = false;
            }

            [Test]
            public void ItShouldSetIssueCorrectly()
            {
                Assert.That(Result.Issue, Is.EqualTo(string.Format("Invalid uri specified for {0}: {1}", Name, TestValue)));
            }
        }

        public class WhenUriEmpty : UriValidatorTests
        {
            protected override void DoCustomSetup()
            {
                TestValue = "";
                ExpectedResult = false;
            }

            [Test]
            public void ItShouldSetIssueCorrectly()
            {
                Assert.That(Result.Issue, Is.EqualTo(string.Format("Invalid uri specified for {0}: [not set]", Name)));
            }
        }
    }
}