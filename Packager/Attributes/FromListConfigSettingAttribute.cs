using System;
using System.Linq;

namespace Packager.Attributes
{
    public class FromListConfigSettingAttributeAttribute : AbstractFromConfigSettingAttribute
    {
        public FromListConfigSettingAttributeAttribute(string name, bool required = true)
            : base(name, required)
        {
        }

        public override object Convert(string value)
        {
            return value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim()).ToArray();
        }
    }
}