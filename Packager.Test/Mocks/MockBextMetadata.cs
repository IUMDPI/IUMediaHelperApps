using System.Linq;
using System.Reflection;
using Packager.Attributes;
using Packager.Models.EmbeddedMetadataModels;

namespace Packager.Test.Mocks
{
    public static class MockBextMetadata
    {
        public static EmbeddedAudioMetadata Get()
        {
            var result = new EmbeddedAudioMetadata();
            foreach (var property in result.GetType().GetProperties().Where(p => p.GetCustomAttribute<BextFieldAttribute>() != null))
            {
                var value = $"{property.Name} value";
                property.SetValue(result, value);
            }

            result.OriginationDate = "2015-10-1";
            result.OriginationTime = "00:00:00";
            result.TimeReference = "0";

            return result;
        }
    }
}