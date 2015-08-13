using System;

namespace Packager.Attributes
{
    public class FromConfigSettingListAttribute : Attribute
    {
        public FromConfigSettingListAttribute(string name, bool required=true)
        {
            Name = name;
            Required = required;
        }

        public string Name { get; private set; }
        public bool Required { get; private set; }
    }
}