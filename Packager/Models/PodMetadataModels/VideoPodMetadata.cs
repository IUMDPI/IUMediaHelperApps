using System.Collections.Generic;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Factories;

namespace Packager.Models.PodMetadataModels
{
    public class VideoPodMetadata : AbstractPodMetadata
    {
        public string ImageFormat { get; set; }

        public string RecordingStandard { get; set; }

        public string Definition { get; set; }

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);
            ImageFormat = factory.ToStringValue(element,"data/object/technical_metadata/image_format");
            RecordingStandard = factory.ToStringValue(element,"data/object/technical_metadata/recording_standard");
            Definition = factory.ToStringValue(element,"data/object/technical_metadata/format_version");
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements,
            IImportableFactory factory)
        {
            var result = new List<AbstractDigitalFile>();
            foreach (var element in elements)
            {
                var file = new DigitalVideoFile();
                file.ImportFromXml(element, factory);
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