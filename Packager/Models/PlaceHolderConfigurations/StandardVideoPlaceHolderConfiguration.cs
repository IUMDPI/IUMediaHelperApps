using System.Collections.Generic;
using Common.Models;
using Packager.Factories;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class StandardVideoPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public StandardVideoPlaceHolderConfiguration() : base(new List<FileUsages> { FileUsages.PreservationMaster,
                FileUsages.ProductionMaster,
                FileUsages.AccessFile}, 
            FileModelFactory.UsageApplications.Video)
        {
        }
    }
}