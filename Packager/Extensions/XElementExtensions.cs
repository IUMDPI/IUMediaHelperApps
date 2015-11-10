using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Deserializers;

namespace Packager.Extensions
{
    public static class XElementExtensions
    {
        public static string GetValue(this XDocument document, string path, string defaultValue)
        {
            return document.XPathSelectElement(path).GetValue(defaultValue);
            
        }

        public static string GetValue(this XElement element, string defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }

            return string.IsNullOrWhiteSpace(element.Value)
                ? defaultValue
                : element.Value;
        }

        public static bool GetValue(this XDocument document, string path, bool defaultValue)
        {
            return document.XPathSelectElement(path).GetValue(defaultValue);
            
        }

        public static bool GetValue(this XElement element, bool defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }

            bool result;
            return bool.TryParse(element.Value, out result)
                ? result
                : defaultValue;
        }

        public static DateTime? GetValue(this XDocument document, string path, DateTime? defaultValue)
        {
            return document.XPathSelectElement(path).GetValue(defaultValue);
        }

        public static DateTime? GetValue(this XElement element, DateTime? defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }

            DateTime result;
            return DateTime.TryParse(element.Value, out result)
                ? result
                : defaultValue;
        }

        public static string BoolValuesToString(this XDocument document, string path, string defaultValue)
        {
            var element = document.XPathSelectElement(path);
            if (element == null)
            {
                return defaultValue;
            }

            var result = element.Elements()
                .Where(d => d.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                .Select(d => d.Name).ToList();

            return result.Any() ? string.Join(",", result) : defaultValue;
        }

        public static List<T> ImportFromChildNodes<T>(this XDocument document, string path) where T : IImportableFromPod, new()
        {
            var element = document.XPathSelectElement(path);
            if (element == null)
            {
                return new List<T>();
            }

            return element.Elements().Select(CreateAndImport<T>).ToList();
        }

        private static T CreateAndImport<T>(XElement parent) where T : IImportableFromPod, new()
        {
            var result = new T();
            result.ImportFromXml(parent);
            return result;
        }
    }
}