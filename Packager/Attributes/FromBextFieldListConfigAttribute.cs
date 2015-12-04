using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Utilities;
using Packager.Utilities.Bext;

namespace Packager.Attributes
{
    namespace Packager.Attributes
    {
        public class FromBextFieldListConfigAttribute : AbstractFromConfigSettingAttribute
        {
            public FromBextFieldListConfigAttribute(string name, bool required = true)
                : base(name, required)
            {
            }

            public override object Convert(string value)
            {
                var values = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim()).ToArray();

                var toSetValues = new List<BextFields>();

                foreach (var rawValue in values)
                {
                    BextFields enumValue;
                    if (Enum.TryParse(rawValue, true, out enumValue) == false)
                    {
                        Issues.Add($"\"{rawValue}\" is not a valid bext field value");
                        continue;
                    }

                    toSetValues.Add(enumValue);
                }

                return toSetValues.ToArray();
            }
        }
    }
}