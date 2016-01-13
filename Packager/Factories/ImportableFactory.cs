using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Extensions;
using Packager.Providers;

namespace Packager.Factories
{
    public interface IImportableFactory
    {
        T ToImportable<T>(XElement element);
        string ToStringValue(XElement parent, string path, string appendIfPresent = "");
        DateTime? ToDateTimeValue(XElement parent, string path);
        bool ToBooleanValue(XElement parent, string path);
        string ResolveTrackConfiguration(XElement element, string path);
        string ResolveTapeThickness(XElement element, string path);
        string ResolvePlaybackSpeed(XElement element, string path);
        string ResolveSoundField(XElement element, string path);
        string ResolveDamage(XElement element, string path);
        string ResolvePreservationProblems(XElement element, string path);

        List<T> ToObjectList<T>(XElement element, string path) where T : IImportable, new();
    }

    public class ImportableFactory : IImportableFactory
    {
        public ImportableFactory(ILookupsProvider lookupsProvider)
        {
            LookupsProvider = lookupsProvider;
        }

        private ILookupsProvider LookupsProvider { get; }

        public bool ToBooleanValue(XElement parent, string path)
        {
            var value = ToStringValue(parent, path);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            bool result;
            return bool.TryParse(value, out result) && result;
        }

        public string ResolveTrackConfiguration(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.TrackConfiguration);
        }

        public string ResolveTapeThickness(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.TapeThickness);
        }

        public string ResolvePlaybackSpeed(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.PlaybackSpeed);
        }

        public string ResolveSoundField(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.SoundField);
        }

        public string ResolveDamage(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.Damage);
        }

        public string ResolvePreservationProblems(XElement element, string path)
        {
            return ToResolvedDelimitedString(element, path, LookupsProvider.PreservationProblem);
        }

        public List<T> ToObjectList<T>(XElement element, string path) where T : IImportable, new()
        {
            if (element == null)
            {
                return new List<T>();
            }

            var results = new List<T>();
            foreach (var childElement in element.Elements(path))
            {
                var instance = new T();
                instance.ImportFromXml(childElement,this);
                results.Add(instance);
            }

            return results;
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

            return string.IsNullOrWhiteSpace(element.Value)
                ? string.Empty
                : element.Value.AppendIfValuePresent(appendIfPresent);
        }

        public DateTime? ToDateTimeValue(XElement parent, string path)
        {
            var value = ToStringValue(parent, path);
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

        private static string ToResolvedDelimitedString(XNode parent, string path,
            IDictionary<string, string> lookupDictionary)
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

            return string.Join(",", result);
        }

        private static string FindValueInDictionary(string value, IDictionary<string, string> dictionary)
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