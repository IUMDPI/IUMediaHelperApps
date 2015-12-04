using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Packager.Attributes
{
    public abstract class AbstractFromConfigSettingAttribute : Attribute
    {
        public AbstractFromConfigSettingAttribute(string name, bool required = true)
        {
            Name = name;
            Required = required;
            Issues = new List<string>();
        }

        public string Name { get; private set; }
        public bool Required { get; private set; }

        public abstract object Convert(string value);

        public List<string> Issues { get; }
    }
}