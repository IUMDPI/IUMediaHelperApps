using System;

namespace Packager.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelFieldAttribute : Attribute
    {
        public ExcelFieldAttribute(string name, bool required)
        {
            Name = name;
            Required = required;
        }

        public string Name { get; private set; }
        public bool Required { get; private set; }
    }
}