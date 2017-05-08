using System;
using System.Xml.Linq;

namespace Packager.Extensions
{
    public static class XElmentExtensions
    {
        public static bool AttributeEquals(this XAttribute attribute, string expectedValue, 
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (attribute == null || attribute.Value.IsNotSet())
            {
                return false;
            }

            return attribute.Value.Equals(expectedValue, comparison);


        }
    }
}
