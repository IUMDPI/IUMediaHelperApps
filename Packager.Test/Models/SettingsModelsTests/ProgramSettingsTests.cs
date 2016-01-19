using System;
using System.Reflection;
using NUnit.Framework;
using Packager.Models.SettingsModels;
using Packager.Validators.Attributes;

namespace Packager.Test.Models.SettingsModelsTests
{
    public class ProgramSettingsTests
    {
        [TestFixture]
        public class AttributesTests : ProgramSettingsTests
        {
            private static void AssertHasAttribute<T>(MemberInfo info) where T : Attribute
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
                var property = typeof (ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateFolderAttribute>(property);
            }

            [TestCase("BwfMetaEditPath")]
            [TestCase("FFMPEGPath")]
            [TestCase("FFProbePath")]
            [TestCase("PodAuthFilePath")]
            public void PropertyShouldHaveValidateFileAttribute(string properyName)
            {
                var property = typeof (ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateFileAttribute>(property);
            }

            [TestCase("WebServiceUrl")]
            public void PropertyShouldHaveValidateUriAttribute(string properyName)
            {
                var property = typeof (ProgramSettings).GetProperty(properyName);
                AssertHasAttribute<ValidateUriAttribute>(property);
            }
        }
    }
}