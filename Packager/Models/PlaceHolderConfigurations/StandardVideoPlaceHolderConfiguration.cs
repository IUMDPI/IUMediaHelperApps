using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class StandardVideoPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public StandardVideoPlaceHolderConfiguration() : base(new List<IFileUsage> {
            FileUsages.PreservationMaster,
            FileUsages.MezzanineFile,
            FileUsages.AccessFile})
        {
        }

        protected override List<AbstractFile> GetPlaceHolders(PlaceHolderFile template)
        {
            return new List<AbstractFile>
            {
                template.ConvertTo<VideoPreservationFile>(),
                template.ConvertTo<MezzanineFile>(),
                template.ConvertTo<AccessFile>()
            };
        }
    }
}