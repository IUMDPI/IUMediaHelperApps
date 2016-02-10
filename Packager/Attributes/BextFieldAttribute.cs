using System;
using System.Reflection;
using Packager.Utilities.Bext;

namespace Packager.Attributes
{
    public class BextFieldAttribute : Attribute
    {
        public BextFieldAttribute(BextFields field, int maxLength = 0)
        {
            Field = field;
            MaxLength = maxLength;
        }

        public BextFields Field { get; }
        public int MaxLength { get; }

        public bool ValueWithinLengthLimit(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            if (MaxLength == 0)
            {
                return true;
            }

            return value.Length <= MaxLength;
        }

        public string GetFFMPEGArgument()
        {
            var type = Field.GetType();
            var name = Enum.GetName(type, Field);
            var attribute = type.GetField(name).GetCustomAttribute<FFMPEGArgumentAttribute>();

            return attribute == null ? Field.ToString() : attribute.Value;
        }
    }
}