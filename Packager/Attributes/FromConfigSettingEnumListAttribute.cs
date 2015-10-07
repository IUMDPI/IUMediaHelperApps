using System;

namespace Packager.Attributes
{
    namespace Packager.Attributes
    {
        public class FromConfigSettingEnumListAttribute : Attribute
        {
            public FromConfigSettingEnumListAttribute(string name, bool required = true)
            {
                Name = name;
                Required = required;
            }

            public string Name { get; private set; }
            public bool Required { get; private set; }
        }
    }
}