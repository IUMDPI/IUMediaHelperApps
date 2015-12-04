using System;

namespace Packager.Attributes
{
    public class FFMPEGArgumentAttribute : Attribute
    {
        public string Value { get; set; }

        public FFMPEGArgumentAttribute(string value)
        {
            Value = value;
        }
    }
}