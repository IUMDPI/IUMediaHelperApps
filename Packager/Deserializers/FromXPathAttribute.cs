using System;
using System.Reflection;

namespace Packager.Deserializers
{
    public class XPathAttribute : Attribute
    {
        public XPathAttribute(string xPath, object defaultValue = null)
        {
            XPath = xPath;
            DefaultValue = defaultValue;
        }

        public string XPath { get; set; }
        public object DefaultValue { get; set; }

        public static bool IsPresent(PropertyInfo info)
        {
            return info.GetCustomAttribute<XPathAttribute>() != null;
        }
    }
}