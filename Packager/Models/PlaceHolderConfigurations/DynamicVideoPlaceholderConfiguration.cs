using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class DynamicVideoPlaceholderConfiguration : AbstractPlaceHolderConfiguration
    {
        public DynamicVideoPlaceholderConfiguration(List<FileUsages> requiredUsages) : base(requiredUsages)
        {
        }

        protected override List<AbstractFile> GetPlaceHolders(PlaceHolderFile template)
        {
            var result = new List<AbstractFile>();
            if (RequiredUsages.Contains(FileUsages.PreservationMaster))
            {
                result.Add(template.ConvertTo<VideoPreservationFile>());
            }

            if (RequiredUsages.Contains(FileUsages.PreservationIntermediateMaster))
            {
                result.Add(template.ConvertTo<VideoPreservationIntermediateFile>());
            }

            if (RequiredUsages.Contains(FileUsages.MezzanineFile))
            {
                result.Add(template.ConvertTo<MezzanineFile>());
            }

            if (RequiredUsages.Contains(FileUsages.AccessFile))
            {
                result.Add(template.ConvertTo<AccessFile>());
            }

            return result;
        }
    }
}