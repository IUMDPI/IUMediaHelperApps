using System;
using System.Linq;

namespace Packager.Attributes
{
    public class FromListConfigSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromListConfigSettingAttribute(string name): base(name)
        {
        }

        public override object Convert(string value)
        {
            return value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim()).ToArray();
        }
    }
}