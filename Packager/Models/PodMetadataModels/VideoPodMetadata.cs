using System.Collections.Generic;
using System.Xml.Linq;

namespace Packager.Models.PodMetadataModels
{
    public class VideoPodMetadata : AbstractPodMetadata
    {
        
        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements)
        {
            var result = new List<AbstractDigitalFile>();
            foreach (var element in elements)
            {
                var file = new DigitalVideoFile();
                file.ImportFromXml(element);
                result.Add(file);
            }

            return result;
        }
    }
}