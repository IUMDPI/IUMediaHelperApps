using System.Linq;
using System.Reflection;
using ICSharpCode.AvalonEdit.Rendering;
using NUnit.Framework;
using Packager.Attributes;
using Packager.Extensions;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Utilities;
using Packager.Utilities.Process;

namespace Packager.Test.Models.BextModels
{
    [TestFixture]
    public class BextMetadataTests
    {
        private EmbeddedAudioMetadata GetMetadata()
        {
            return new EmbeddedAudioMetadata
            {
                CodingHistory = "Coding History",
                Description = "Description",
                IARL = "IARL",
                IART = "IART",
                ICMS = "ICMS",
                ICMT = "ICMT",
                ICOP = "ICOP",
                ICRD = "ICRD",
                IENG = "IENG",
                IGNR = "IGNR",
                IKEY = "IKEY",
                IMED = "IMED",
                INAM = "INAM",
                IPRD = "IPRD",
                ISBJ = "ISBJ",
                ISFT = "ISFT",
                ISRC = "ISRC",
                ITCH = "ITCH",
                ISRF = "ISRF",
                OriginationDate = "2015-01-01",
                OriginatorReference = "Originator Reference",
                TimeReference = "0",
                OriginationTime = "00:00:00",
                Originator = "Originator",
                UMID = "UMID"
            };
        }



        private static void ConfirmArgumentsPresentAndCorrect(EmbeddedAudioMetadata metadata, ArgumentBuilder arguments)
        {
            foreach (var property in metadata.GetType().GetProperties())
            {
                var attribute = property.GetCustomAttribute<BextFieldAttribute>();
                Assert.That(attribute, Is.Not.Null, $"{property.Name} should have bext field attribute");

                var value = property.GetValue(metadata).ToString();
                var expectedEntry = $"-metadata {attribute.GetFFMPEGArgument()}={value.NormalizeForCommandLine().ToQuoted()}";
                if (string.IsNullOrWhiteSpace(value))
                {
                    Assert.That(arguments.SingleOrDefault(a => a.Equals(expectedEntry)), Is.Null, 
                        $"arguments should not contain entry for empty property {property.Name}");
                }
                else
                {
                    Assert.That(arguments.SingleOrDefault(a => a.Equals(expectedEntry)), Is.Not.Null, 
                        $"arguments should contain correct entry for property {property.Name}");
                }
            }
        }

        [Test]
        public void AsArgumentsShouldNotNotIncludePropertiesWithoutValues()
        {
            var metadata = GetMetadata();
            
            // clear some fields
            metadata.IARL = "";
            metadata.ICMS = "";
            metadata.IART = "";

            var result = metadata.AsArguments();
            ConfirmArgumentsPresentAndCorrect(metadata, result);
        }

        [Test]
        public void AsArgumentsShouldContainAllFieldsWithValues()
        {
            var metadata = GetMetadata();
            var result = metadata.AsArguments();
            ConfirmArgumentsPresentAndCorrect(metadata, result);
        }

        [Test]
        public void AllPropertiesShouldHaveBextFieldAttribute()
        {
            foreach (var property in typeof(EmbeddedAudioMetadata).GetProperties())
            {
                var attribute = property.GetCustomAttribute<BextFieldAttribute>();
                Assert.That(attribute, Is.Not.Null, $"{property.Name} should have bext field attribute");
            }
        }
    }
}