using System;

namespace Packager.Attributes
{
    public class FFMPEGArgumentAttribute : Attribute
    {
        public string Value { get; }

        public FFMPEGArgumentAttribute(string value)
        {
            Value = value;
        }
    }
}