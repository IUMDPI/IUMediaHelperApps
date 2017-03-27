using System.Collections.Generic;
using Common.Models;
using Packager.Factories;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class PresIntAudioPlaceHolderConfiguration : AbstractPlaceHolderConfiguration
    {
        public PresIntAudioPlaceHolderConfiguration() : base(new List<FileUsages> { FileUsages.PreservationMaster,
            FileUsages.PreservationIntermediateMaster,
            FileUsages.ProductionMaster,
            FileUsages.AccessFile}, FileModelFactory.UsageApplications.Audio)
        {
        }
    }
}