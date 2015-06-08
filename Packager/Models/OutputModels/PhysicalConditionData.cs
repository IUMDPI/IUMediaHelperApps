using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class PhysicalConditionData
    {
        public string Damage { get; set; }
        public string PreservationProblem { get; set; }
    }
}