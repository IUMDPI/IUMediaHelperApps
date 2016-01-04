using System.Collections.Generic;
using System.Xml.Linq;
using Packager.Providers;

namespace Packager.Models.PodMetadataModels
{
    public class VideoPodMetadata : AbstractPodMetadata
    {
        
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