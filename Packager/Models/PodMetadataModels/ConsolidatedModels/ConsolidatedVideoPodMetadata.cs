using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Extensions;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class ConsolidatedVideoPodMetadata : AbstractConsolidatedPodMetadata
    {
        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(IEnumerable<DigitalFileProvenance> originals)
        {
            return originals.Select(provenance => new ConsolidatedDigitalVideoFile(provenance))
                    .Cast<AbstractConsolidatedDigitalFile>().ToList();
        }

        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(XDocument document)
        {
            return document.ImportFromChildNodes<ConsolidatedDigitalVideoFile>("/pod/data/object/digital_provenance/digital_files")
                .Select(e => e as AbstractConsolidatedDigitalFile).ToList();
        }
    }
}