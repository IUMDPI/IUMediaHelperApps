using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Factories;

namespace Packager.Models.PodMetadataModels
{
    public class VideoPodMetadata : AbstractPodMetadata
    {
        public string ImageFormat { get; set; }

        public string RecordingStandard { get; set; }

        public string FormatVersion { get; set; }

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);
            ImageFormat = factory.ToStringValue(element, "data/image_format");
            RecordingStandard = factory.ToStringValue(element, "data/recording_standard");
            FormatVersion = factory.ToStringValue(element, "data/object/technical_metadata/format_version");
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(XElement element, string path,
            IImportableFactory factory)
        {
            return factory.ToObjectList<DigitalVideoFile>(element, path)
                .Cast<AbstractDigitalFile>().ToList();
        }
    }
}