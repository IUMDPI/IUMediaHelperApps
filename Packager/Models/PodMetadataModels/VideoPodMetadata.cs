using System.Collections.Generic;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Providers;

namespace Packager.Models.PodMetadataModels
{
    public class VideoPodMetadata : AbstractPodMetadata
    {
        public string ImageFormat { get; set; }

        public string RecordingStandard { get; set; }

        public string Definition { get; set; }

        public override void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            base.ImportFromXml(element, lookupsProvider);
            ImageFormat = element.ToStringValue("data/object/technical_metadata/image_format");
            RecordingStandard = element.ToStringValue("data/object/technical_metadata/recording_standard");
            Definition = element.ToStringValue("data/object/technical_metadata/format_version");
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements, ILookupsProvider lookupsProvider)
        {
            var result = new List<AbstractDigitalFile>();
            foreach (var element in elements)
            {
                var file = new DigitalVideoFile();
                file.ImportFromXml(element, lookupsProvider);
                result.Add(file);
            }

            return result;
        }

        protected override void NormalizeFileProvenances()
        {
            // ignore
        }
    }
}