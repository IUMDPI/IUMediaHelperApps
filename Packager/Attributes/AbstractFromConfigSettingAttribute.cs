using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Packager.Attributes
{
    public abstract class AbstractFromConfigSettingAttribute : Attribute
    {
        protected AbstractFromConfigSettingAttribute(string name)
        {
            Name = name;
            Issues = new List<string>();
        }

        public string Name { get; private set; }
       
        public abstract object Convert(string value);

        public List<string> Issues { get; }
    }
}