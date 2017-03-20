using System.Collections.Generic;
using System.Linq;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Utilities.PlaceHolderGenerators
{
    public abstract class AbstractPlaceHolderGenerator : IPlaceHolderGenerator
    {
        public List<AbstractFile> GetPlaceHoldersToAdd(List<AbstractFile> fileModels)
        {
            var sequencesRequiringPlaceHolders = GetSequencesRequringPlaceHolders(fileModels);
            if (sequencesRequiringPlaceHolders.Any() == false)
            {
                return new List<AbstractFile>();
            }

            var projectCode = GetProjectCode(fileModels);
            var barcode = GetBarCode(fileModels);

            var result = new List<AbstractFile>();
            return sequencesRequiringPlaceHolders
                .Aggregate(result, (current, sequence) => 
                    current.Concat(
                            GetPlaceHoldersForSequenceIndicator(projectCode, barcode, sequence))
                        .ToList());
        }

        protected abstract List<int> GetSequencesRequringPlaceHolders(IEnumerable<AbstractFile> fileModels);

        protected abstract IEnumerable<AbstractFile> GetPlaceHoldersForSequenceIndicator(string projectCode,
            string barCode, int sequenceIndicator);
        
        private static string GetProjectCode(IEnumerable<AbstractFile> fileModels)
        {
            var firstPresFile = fileModels.FirstOrDefault(f => f.IsPreservationVersion());
            return firstPresFile?.ProjectCode;
        }

        private static string GetBarCode(IEnumerable<AbstractFile> fileModels)
        {
            var firstPresFile = fileModels.FirstOrDefault(f => f.IsPreservationVersion());
            return firstPresFile?.BarCode;
        }
    }
}