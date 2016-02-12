using System;
using Packager.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class PhysicalConditionData
    {
        public string Damage { get; set; }
        public string PreservationProblem { get; set; }

        public bool ShouldSerializePreservationProblem()
        {
            return PreservationProblem.IsSet();
        }
    }
}