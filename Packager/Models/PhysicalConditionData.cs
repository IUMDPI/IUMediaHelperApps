using Packager.Attributes;

namespace Packager.Models
{
    public class PhysicalConditionData
    {
        [ExcelField("PhysicalCondition-Damage", true)]
        public string Damage { get; set; }

        [ExcelField("PhysicalCondition-PreservationProblem", true)]
        public string PreservationProblem { get; set; }
    }
}