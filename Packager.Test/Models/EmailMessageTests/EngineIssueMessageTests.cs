using System;
using NUnit.Framework;
using Packager.Models.EmailMessageModels;

namespace Packager.Test.Models.EmailMessageTests
{
    [TestFixture]
    public class EngineIssueMessageTests
    {
        [SetUp]
        public void BeforeEach()
        {
            Exception = new Exception("[issue message]", new Exception("[inner exception]"));
            Message = new EngineIssueMessage(
                new[] {"to1@test.org", "to2@test.org"},
                "from@test.org",
                new[] {"path1", "path2"},
                "[machine name]", Exception);
        }

        private EngineIssueMessage Message { get; set; }
        private Exception Exception { get; set; }

        [Test]
        public void FromShouldBeCorrect()
        {
            Assert.That(Message.From, Is.EqualTo("from@test.org"));
        }

        [Test]
        public void ToAddressesShouldBeCorrect()
        {
            Assert.That(Message.ToAddresses, Is.EquivalentTo(new[] { "to1@test.org", "to2@test.org" }));
        }

        [Test]
        public void AttachmentsShouldBeCorrect()
        {
            Assert.That(Message.Attachments, Is.EquivalentTo(new[] { "path1", "path2" }));
        }

        [Test]
        public void TitleShouldBeCorrect()
        {
            Assert.That(Message.Title, Is.EqualTo("Issue occurred while processing objects on workstation [machine name]"));
        }

        [Test]
        public void BodyShouldStartWithHeader()
        {
            Assert.That(Message.Body.StartsWith("<html><body>"));
        }

        [Test]
        public void BodyShouldHaveFirstParagraph()
        {
            Assert.That(Message.Body.Contains("<p>An issue occurred while processing objects on workstation [machine name]:</p>"));
        }

        [Test]
        public void BodyShouldHaveSecondParagraph()
        {
            Assert.That(Message.Body.Contains("<blockquote>[issue message]</blockquote>"));
        }

        [Test]
        public void BodyShouldHaveThirdParagraph()
        {
            Assert.That(Message.Body.Contains("<p>Stack trace:</p>"));
        }

        [Test]
        public void BodyShouldHaveFourthParagraph()
        {
            Assert.That(Message.Body.Contains($"<p><pre>{Exception.StackTrace}</pre></p>"));
        }

        [Test]
        public void BodyShouldContainFooter()
        {
            Assert.That(Message.Body.Contains("</body></html>"));
        }
    }
}