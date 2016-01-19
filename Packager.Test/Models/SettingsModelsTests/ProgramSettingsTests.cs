using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.SettingsModels;
using Packager.Validators.Attributes;

namespace Packager.Test.Models.SettingsModelsTests
{
    
    public class ProgramSettingsTests
    {
        [TestFixture]
        public class AttributesTests : ProgramSettingsTests
        {
            private static void AssertHasAttribute<T>(MemberInfo info) where T:Attribute
            {
                Assert.That(info, Is.Not.Null);
                Assert.That(info.GetCustomAttribute<T>(), Is.Not.Null);
            }

            [TestCase("FFMPEGAudioProductionArguments")]
            [TestCase("FFMPEGAudioAccessArguments")]
            [TestCase("FFMPEGVideoMezzanineArguments")]
            [TestCase("FFMPEGVideoAccessArguments")]
            [TestCase("FFProbeVideoQualityControlArguments")]
            [TestCase("ProjectCode")]
            [TestCase("UnitPrefix")]
            public void PropertyShouldHaveRequiredAttribute(string properyName)
            {
                var property = typeof (ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<RequiredAttribute>(property);
            }

            [TestCase("InputDirectory")]
            [TestCase("ProcessingDirectory")]
            [TestCase("DropBoxDirectoryName")]
            [TestCase("ErrorDirectoryName")]
            [TestCase("SuccessDirectoryName")]
            [TestCase("LogDirectoryName")]
            public void PropertyShouldHaveValidateFolderAttribute(string properyName)
            {
                var property = typeof(ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateFolderAttribute>(property);
            }

            [TestCase("BwfMetaEditPath")]
            [TestCase("FFMPEGPath")]
            [TestCase("FFProbePath")]
            [TestCase("PodAuthFilePath")]
            public void PropertyShouldHaveValidateFileAttribute(string properyName)
            {
                var property = typeof(ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateFileAttribute>(property);
            }

            [TestCase("WebServiceUrl")]
            public void PropertyShouldHaveValidateUriAttribute(string properyName)
            {
                var property = typeof(ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateUriAttribute>(property);
            }
        }
   
        [TestFixture]
        public class WhenImporting : ProgramSettingsTests
        {
            private ISettingsFactory Factory { get; set; }
            private NameValueCollection Dictionary { get; set; }
            private string[] MockAddressList { get; set; }
            private ProgramSettings Instance { get; set; }
            [SetUp]
            public void BeforeEach()
            {
                Dictionary = new NameValueCollection();
                Factory = Substitute.For<ISettingsFactory>();

                Factory.GetStringValue(Dictionary, Arg.Any<string>()).Returns(x => $"{x.Arg<string>()} value");
                Factory.GetIntValue(Dictionary, "DeleteProcessedAfterInDays",0).Returns(8);

                MockAddressList = new[] {"address1", "address2"};
                Factory.GetStringValues(Dictionary, "IssueNotifyEmailAddresses").Returns(MockAddressList);

                Instance = new ProgramSettings();
                Instance.Import(Dictionary, Factory);
            }

            private void AssertPropertyEquals(string propertyName, string expected)
            {
                var info = typeof (ProgramSettings).GetProperty(propertyName);
                Assert.That(info, Is.Not.Null);

                Assert.That(info.GetValue(Instance), Is.EqualTo(expected));
            }

            [TestCase("BwfMetaEditPath", "PathToMetaEdit")]
            [TestCase("FFMPEGPath", "PathToFFMpeg")]
            [TestCase("FFProbePath", "PathToFFProbe")]
            [TestCase("InputDirectory", "WhereStaffWorkDirectoryName")]
            [TestCase("FFMPEGAudioProductionArguments", "ffmpegAudioProductionArguments")]
            [TestCase("FFMPEGAudioAccessArguments", "ffmpegAudioAccessArguments")]
            [TestCase("FFMPEGVideoMezzanineArguments", "ffmpegVideoMezzanineArguments")]
            [TestCase("FFMPEGVideoAccessArguments", "ffmpegVideoAccessArguments")]
            [TestCase("FFProbeVideoQualityControlArguments", "ffprobeVideoQualityControlArguments")]
            [TestCase("ProjectCode", "ProjectCode")]
            [TestCase("DropBoxDirectoryName", "DropBoxDirectoryName")]
            [TestCase("ErrorDirectoryName", "ErrorDirectoryName")]
            [TestCase("SuccessDirectoryName", "SuccessDirectoryName")]
            [TestCase("LogDirectoryName", "LogDirectoryName")]
            [TestCase("UnitPrefix", "UnitPrefix")]
            [TestCase("SmtpServer", "SmtpServer")]
            [TestCase("WebServiceUrl", "WebServiceUrl")]
            [TestCase("PodAuthFilePath", "PodAuthorizationFile")]
            [TestCase("FromEmailAddress", "FromEmailAddress")]
            public void ItShouldCallFactoryCorrectlyToSetStringValues(string propertyName, string dictionaryKey)
            {
                Factory.Received().GetStringValue(Dictionary,dictionaryKey);
                AssertPropertyEquals(propertyName, $"{dictionaryKey} value");
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToSetDeleteSuccessfulObjectsAfterDays()
            {
                Factory.Received().GetIntValue(Dictionary, "DeleteProcessedAfterInDays", 0);
                Assert.That(Instance.DeleteSuccessfulObjectsAfterDays, Is.EqualTo(8));
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToSetIssueNotifyEmailAddresses()
            {
                Factory.Received().GetStringValues(Dictionary, "IssueNotifyEmailAddresses");
                Assert.That(Instance.IssueNotifyEmailAddresses, Is.EquivalentTo(MockAddressList));
            }

        }
    }
}
