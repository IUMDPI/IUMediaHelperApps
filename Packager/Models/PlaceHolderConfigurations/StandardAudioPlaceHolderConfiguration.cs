using System.Collections.Generic;
using Common.Models;
using Packager.Factories;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class StandardAudioPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public StandardAudioPlaceHolderConfiguration() : base(new List<FileUsages> { FileUsages.PreservationMaster,
                FileUsages.ProductionMaster,
                FileUsages.AccessFile},
            FileModelFactory.UsageApplications.Audio)
        {
        }
    }
}