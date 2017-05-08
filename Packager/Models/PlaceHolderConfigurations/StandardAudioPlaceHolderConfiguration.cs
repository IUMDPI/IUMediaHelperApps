using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class StandardAudioPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public StandardAudioPlaceHolderConfiguration() : base(
            new List<IFileUsage> {
                FileUsages.PreservationMaster,
                FileUsages.ProductionMaster,
                FileUsages.AccessFile})
        {
        }

        protected override List<AbstractFile> GetPlaceHolders(PlaceHolderFile template)
        {
            return new List<AbstractFile>
            {
                template.ConvertTo<AudioPreservationFile>(),
                template.ConvertTo<ProductionFile>(),
                template.ConvertTo<AccessFile>()
            };
        }
    }
}