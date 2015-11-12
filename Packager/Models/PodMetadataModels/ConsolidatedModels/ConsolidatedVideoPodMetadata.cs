using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class ConsolidatedVideoPodMetadata : AbstractConsolidatedPodMetadata
    {
        
        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements)
        {
            var result = new List<AbstractConsolidatedDigitalFile>();
            foreach (var element in elements)
            {
                var file = new ConsolidatedDigitalVideoFile();
                file.ImportFromXml(element);
                result.Add(file);
            }

            return result;
        }
    }
}