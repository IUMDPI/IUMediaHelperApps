using System;
using Packager.Utilities;

namespace Packager.Attributes
{
    public class BextFieldAttribute : Attribute
    {
        public BextFieldAttribute(BextFields field, int maxLength = 0)
        {
            Field = field;
            MaxLength = maxLength;
        }

        public BextFields Field { get; set; }
        public int MaxLength { get; set; }

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
    }
}