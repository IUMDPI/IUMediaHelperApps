using System.Collections.Generic;
using NUnit.Framework;
using Packager.Validators;

namespace Packager.Test.Validators
{
    [TestFixture]
    public class HasMembersValidatorTests
    {
        [SetUp]
        public void BeforeEach()
        {
            DoCustomSetup();
            Result = new MembersValidator().Validate(Subject, "Subject");
        }

        private ValidationResult Result { get; set; }

        private bool ExpectedResult { get; set; }

        private object Subject { get; set; }

        protected virtual void DoCustomSetup()
        {
            Subject = new List<string> {"value"};
            ExpectedResult = true;
        }

        public class WhenNoMembersPresent : HasMembersValidatorTests
        {
            protected override void DoCustomSetup()
            {
                Subject = new List<string>();
                ExpectedResult = false;
            }
        }

        public class WhenCollectionNull : HasMembersValidatorTests
        {
            protected override void DoCustomSetup()
            {
                Subject = null;
                ExpectedResult = false;
            }
        }

        public class WhenCollectionIsArray : HasMembersValidatorTests
        {
            protected override void DoCustomSetup()
            {
                Subject = new[] {"test"};
                ExpectedResult = true;
            }
        }

        [Test]
        public void ItShouldReturnCorrectResult()
        {
            Assert.That(Result.Result, Is.EqualTo(ExpectedResult));
        }

        [Test]
        public void ResultMessageShouldBeCorrect()
        {
            var expected = ExpectedResult
                ? ""
                : "Subject must have at least one member";

            Assert.That(Result.Issue, Is.EqualTo(expected));
        }
    }
}