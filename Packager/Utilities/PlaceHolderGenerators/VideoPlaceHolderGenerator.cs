using System.Collections.Generic;
using System.Linq;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Utilities.PlaceHolderGenerators
{
    public class VideoPlaceHolderGenerator: AbstractPlaceHolderGenerator
    {
        protected override List<int> GetSequencesRequringPlaceHolders(IEnumerable<AbstractFile> fileModels)
        {
            return fileModels
                .GroupBy(f => f.SequenceIndicator) // group files by sequence id
                .Where(g => // and find sequence groupings
                    g.All(f => f.IsAccessVersion() == false) && // with no access entries
                    g.All(f => f.IsPreservationVersion() == false) && // and with no pres entries 
                    g.All(f => f.IsMezzanineVersion() == false)) // and with no prod entries
                .Select(g => g.Key)
                .ToList();
        }

        protected override IEnumerable<AbstractFile> GetPlaceHoldersForSequenceIndicator(string projectCode, string barCode, int sequenceIndicator)
        {
            var template = new PlaceHolderFile(projectCode, barCode, sequenceIndicator);
            return new List<AbstractFile>
            {
                new VideoPreservationFile(template),
                new MezzanineFile(template),
                new AccessFile(template)
            };
        }
    }
}