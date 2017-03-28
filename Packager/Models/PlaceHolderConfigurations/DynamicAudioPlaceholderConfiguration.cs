using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class DynamicAudioPlaceholderConfiguration:AbstractPlaceHolderConfiguration
    {
        public DynamicAudioPlaceholderConfiguration(List<FileUsages> requiredUsages) : base(requiredUsages)
        {
        }

        protected override List<AbstractFile> GetPlaceHolders(PlaceHolderFile template)
        {
            var result = new List<AbstractFile>();
            if (RequiredUsages.Contains(FileUsages.PreservationMaster))
            {
                result.Add(template.ConvertTo<AudioPreservationFile>());
            }

            if (RequiredUsages.Contains(FileUsages.PreservationIntermediateMaster))
            {
                result.Add(template.ConvertTo<AudioPreservationIntermediateFile>());
            }

            if (RequiredUsages.Contains(FileUsages.ProductionMaster))
            {
                result.Add(template.ConvertTo<ProductionFile>());
            }

            if (RequiredUsages.Contains(FileUsages.AccessFile))
            {
                result.Add(template.ConvertTo<AccessFile>());
            }

            return result;
        }
    }
}
