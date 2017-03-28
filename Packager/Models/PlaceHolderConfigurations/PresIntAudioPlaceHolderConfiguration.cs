using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class PresIntAudioPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public PresIntAudioPlaceHolderConfiguration() : base(new List<FileUsages> { FileUsages.PreservationMaster,
            FileUsages.PreservationIntermediateMaster,
            FileUsages.ProductionMaster,
            FileUsages.AccessFile})
        {
        }

        protected override List<AbstractFile> GetPlaceHolders(PlaceHolderFile template)
        {
            return new List<AbstractFile>
            {
                template.ConvertTo<AudioPreservationFile>(),
                template.ConvertTo<AudioPreservationIntermediateFile>(),
                template.ConvertTo<ProductionFile>(),
                template.ConvertTo<AccessFile>()
            };
        }
    }
}