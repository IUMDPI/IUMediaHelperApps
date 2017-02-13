using System.Collections.Specialized;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class SettingsFactoryTests
    {
        [TestFixture]
        public class GetValueTests : SettingsFactoryTests
        {
            [TestCase("value 1", "value 1")]
            [TestCase("value 2", "value 2")]
            [TestCase("", "")]
            [TestCase(" ", "")]
            [TestCase("   ", "")]
            public void GetStringValueShouldReturnCorrectValue(string rawValue, string expected)
            {
                var dictionary = new NameValueCollection
                {
                    {"key", rawValue}
                };

                Assert.That(SettingsFactory.GetStringValue(dictionary, "key"), Is.EqualTo(expected));
            }

            [TestCase("1", 1)]
            [TestCase("2", 2)]
            [TestCase("-1", -1)]
            public void GetIntValueShouldReturnCorrectValue(string rawValue, int expected)
            {
                var dictionary = new NameValueCollection
                {
                    {"key", rawValue}
                };

                Assert.That(SettingsFactory.GetIntValue(dictionary, "key", -100), Is.EqualTo(expected));
            }

            [TestCase("")]
            [TestCase(" ")]
            [TestCase("invalid")]
            public void GetIntValueShouldReturnDefaultValueIfRawValueInvalid(string rawValue)
            {
                var dictionary = new NameValueCollection
                {
                    {"key", rawValue}
                };

                Assert.That(SettingsFactory.GetIntValue(dictionary, "key", -100), Is.EqualTo(-100));
            }

            [TestCase("value1,value2,value3")]
            [TestCase("value1, value2, value3")]
            [TestCase("value1, value2, value3, ,")]
            public void GetStringValuesShouldReturnCorrectValue(string rawValue)
            {
                var expected = new[] {"value1", "value2", "value3"};
                var dictionary = new NameValueCollection
                {
                    {"key", rawValue}
                };

                Assert.That(SettingsFactory.GetStringValues(dictionary, "key"), Is.EquivalentTo(expected));
            }
        }

        [TestFixture]
        public class WhenImporting : SettingsFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                var settings = new NameValueCollection
                {
                    {"PathToMetaEdit", "PathToMetaEdit value"},
                    {"PathToFFMpeg", "PathToFFMpeg value"},
                    {"PathToFFProbe", "PathToFFProbe value"},
                    {"WhereStaffWorkDirectoryName", "WhereStaffWorkDirectoryName value"},
                    {"ffmpegAudioProductionArguments", "ffmpegAudioProductionArguments value"},
                    {"ffmpegAudioAccessArguments", "ffmpegAudioAccessArguments value"},
                    {"ffmpegVideoMezzanineArguments", "ffmpegVideoMezzanineArguments value"},
                    {"ffmpegVideoAccessArguments", "ffmpegVideoAccessArguments value"},
                    {"ffprobeVideoQualityControlArguments", "ffprobeVideoQualityControlArguments value"},
                    {"ProjectCode", "ProjectCode value"},
                    {"DropBoxDirectoryName", "DropBoxDirectoryName value"},
                    {"ErrorDirectoryName", "ErrorDirectoryName value"},
                    {"SuccessDirectoryName", "SuccessDirectoryName value"},
                    {"LogDirectoryName", "LogDirectoryName value"},
                    {"UnitPrefix", "UnitPrefix value"},
                    {"SmtpServer", "SmtpServer value"},
                    {"WebServiceUrl", "WebServiceUrl value"},
                    {"PodAuthorizationFile", "PodAuthorizationFile value"},
                    {"FromEmailAddress", "FromEmailAddress value"},
                    {"IssueNotifyEmailAddresses", "address1,address2,address3"},
                    {"DeleteProcessedAfterInDays", "8"},
                    {"ImageDirectory", "ImageDirectory value" }
                };

                Result = SettingsFactory.Import(settings);
            }

            private IProgramSettings Result { get; set; }

            private void AssertPropertyEquals(string propertyName, string expected)
            {
                var info = typeof(ProgramSettings).GetProperty(propertyName);
                Assert.That(info, Is.Not.Null);

                Assert.That(info.GetValue(Result), Is.EqualTo(expected));
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
            [TestCase("ImageDirectory", "ImageDirectory")]
            public void StringValuesShouldBeSetCorrectly(string propertyName, string dictionaryKey)
            {
                AssertPropertyEquals(propertyName, $"{dictionaryKey} value");
            }

            [Test]
            public void DeleteSuccessfulObjectsAfterDaysShouldBeSetCorrectly()
            {
                Assert.That(Result.DeleteSuccessfulObjectsAfterDays, Is.EqualTo(8));
            }

            [Test]
            public void IssueNotifyEmailAddressesShouldBeSetCorrectly()
            {
                var expected = new[] {"address1", "address2", "address3"};
                Assert.That(Result.IssueNotifyEmailAddresses, Is.EquivalentTo(expected));
            }
        }
    }
}