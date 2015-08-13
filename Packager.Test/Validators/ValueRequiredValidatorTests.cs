using NUnit.Framework;
using Packager.Validators;

namespace Packager.Test.Validators
{
    [TestFixture]
    public class ValueRequiredValidatorTests
    {
        private const string Name = "Test name";
        private string TestValue { get; set; }
        
        private ValidationResult Result { get; set; }

        protected virtual void DoCustomSetup()
        {
            TestValue = "Value present";
        }

        [SetUp]
        public void BeforeEach()
        {
            

            DoCustomSetup();

            var validator = new ValueRequiredValidator();
            Result = validator.Validate(TestValue, Name);
        }
        
        [Test]
        public void ItShouldReturnCorrectResult()
        {
            Assert.That(Result.Result, Is.EqualTo(!string.IsNullOrWhiteSpace(TestValue)));
        }

        public class WhenThingsGoWrong : ValueRequiredValidatorTests
        {
            protected override void DoCustomSetup()
            {
                TestValue = "";
            }

            [Test]
            public void ItShouldSetIssueCorrectly()
            {
                Assert.That(Result.Issue, Is.EqualTo(string.Format("Value not set for {0}", Name)));
            }
        }
    }
}