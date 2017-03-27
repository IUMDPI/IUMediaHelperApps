using System.Collections.Generic;
using Common.Models;
using Packager.Factories;

namespace Packager.Models.PlaceHolderConfigurations
{
    public class DynamicPlaceholderConfiguration:AbstractPlaceHolderConfiguration
    {
        public DynamicPlaceholderConfiguration(List<FileUsages> requiredUsages, FileModelFactory.UsageApplications application) : base(requiredUsages, application)
        {
        }
    }
}
