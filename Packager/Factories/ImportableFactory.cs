using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Extensions;

namespace Packager.Factories
{
    public interface IImportableFactory
    {
        T ToImportable<T>(XElement element);
        string ToStringValue(XElement parent, string path, string appendIfPresent = "");
        DateTime? ToDateTimeValue(XElement parent, string path);
        bool ToBooleanValue(XElement parent, string path);
        List<T> ToObjectList<T>(XElement element, string path) where T : IImportable, new();
    }

    public class ImportableFactory : IImportableFactory
    {
        public DateTime? ToDateTimeValue(XElement parent, string path)
        {
            var value = ToStringValue(parent, path);
            if (value.IsNotSet())
            {
                return null;
            }

            DateTime result;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result) ==
                false)
            {
                return null;
            }

            return result;
        }

        public bool ToBooleanValue(XElement parent, string path)
        {
            var value = ToStringValue(parent, path);
            if (value.IsNotSet())
            {
                return false;
            }

            bool result;
            return bool.TryParse(value, out result) && result;
        }

        public List<T> ToObjectList<T>(XElement element, string path) where T : IImportable, new()
        {
            if (element == null)
            {
                return new List<T>();
            }

            return Enumerable.ToList(element.XPathSelectElements(path)
                .Select(ToImportable<T>));
        }

        public T ToImportable<T>(XElement element)
        {
            var result = (IImportable) Activator.CreateInstance<T>();
            result.ImportFromXml(element, this);
            return (T) result;
        }

        public string ToStringValue(XElement parent, string path, string appendIfPresent = "")
        {
            var element = parent.XPathSelectElement(path);
            if (element == null)
            {
                return string.Empty;
            }

            return element.Value.IsNotSet()
                ? string.Empty
                : element.Value.AppendIfValuePresent(appendIfPresent);
        }
    }
}