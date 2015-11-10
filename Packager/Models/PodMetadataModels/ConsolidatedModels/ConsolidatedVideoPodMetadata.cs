using System.Collections.Generic;
using System.Linq;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class ConsolidatedVideoPodMetadata : AbstractConsolidatedPodMetadata
    {
        protected override List<AbstractConsolidatedDigitalFile> ImportFileProvenances(IEnumerable<DigitalFileProvenance> originals)
        {
            return originals.Select(provenance => new ConsolidatedDigitalVideoFile(provenance))
                    .Cast<AbstractConsolidatedDigitalFile>().ToList();
        }
    }
}