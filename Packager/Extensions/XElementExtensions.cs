using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Deserializers;
using Packager.Exceptions;
using Packager.Providers;

namespace Packager.Extensions
{
    public static class XElementExtensions
    {
        public static string ToStringValue(this XElement parent, string path)
        {
            var element = parent.XPathSelectElement(path);
            if (element == null)
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(element.Value)
                ? string.Empty
                : element.Value;
        }

        public static bool ToBooleanValue(this XElement parent, string path)
        {
            var value = parent.ToStringValue(path);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            bool result;
            return bool.TryParse(value, out result) && result;
        }

        public static DateTime? ToDateTimeValue(this XElement parent, string path)
        {
            var value = parent.ToStringValue(path);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            DateTime result;
            if (DateTime.TryParse(value, out result) == false)
            {
                return null;
            }

            return result;
        }

        public static string ToResolvedDelimitedString(this XElement parent, string path,
            Dictionary<string, string> lookupDictionary)
        {
            var result = new List<string>();
            var element = parent.XPathSelectElement(path);
            if (element == null)
            {
                return string.Empty;
            }

            // if there are no children, try to match
            // value assigned in dictionary to known lookup value
            // this ensures that the value is normalized correctly
            if (!element.HasElements)
            {
                return FindValueInDictionary(element.Value, lookupDictionary);
            }

            foreach (var key in element.Elements()
                .Where(e => e.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                .Select(e => e.Name.LocalName))
            {
                string value;
                if (lookupDictionary.TryGetValue(key, out value) == false)
                {
                    return string.Empty;
                }

                result.Add(value);
            }
            ;

            return string.Join(",", result);
        }

        private static string FindValueInDictionary(string value, Dictionary<string, string> dictionary)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var dictionaryValue =
                dictionary.Values.FirstOrDefault(v => v.ToLowerInvariant().Equals(value.ToLowerInvariant()));

            return string.IsNullOrWhiteSpace(dictionaryValue) == false ? dictionaryValue : string.Empty;
        }
    }
}