using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        DateTime? ToUtcDateTimeValue(XElement parent, string path);
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

            return element.XPathSelectElements(path)
                .Select(ToImportable<T>).ToList();
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

        public DateTime? ToUtcDateTimeValue(XElement parent, string path)
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

            return result.ToUniversalTime();
        }

        /// <summary>
        /// Converts a POD boolean-child not to comma-delimited list of resolved values
        /// </summary>
        /// <param name="parent">parent XElement node</param>
        /// <param name="path">XPath to get list of child nodes to process</param>
        /// <param name="lookupDictionary">Dictionary with key value pairs used to resolve values</param>
        /// <returns>Comma-delimited string with resolved values or empty string if something goes wrong</returns>
        private static string ToResolvedDelimitedString(XNode parent, string path,
            IDictionary<string, string> lookupDictionary)
        {
            var result = new List<string>();

            // first get the element from the parent using XPath 
            var element = parent.XPathSelectElement(path);
            
            // if no element exists, return empty string
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

            // iterate over the local names of each element that has a 
            // value of 'true'
            foreach (var key in element.Elements()
                .Where(e => e.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                .Select(e => e.Name.LocalName))
            {
                // try to get the value from the lookup dictionary associated with the local name
                string value;
                if (lookupDictionary.TryGetValue(key, out value) == false)
                {
                    // if local name not associated with value return empty string. 
                    // We don't want to include a partial list of value. Empty string
                    // result will raise exception if value is required
                    return string.Empty;
                }

                // add value to results list
                result.Add(value);
            }

            // return results list as comma delimited string
            return string.Join(",", result);
        }

        /// <summary>
        /// Finds the first value in a dictionary that matches regardless of case. Using
        /// this method will ensure that the result is equal to the value in the dictionay
        /// </summary>
        /// <param name="value">The value in question</param>
        /// <param name="dictionary">The dictionary in question</param>
        /// <returns>The matching dictionary value (if found) or an empty string (if not found)</returns>
        private static string FindValueInDictionary(string value, IDictionary<string, string> dictionary)
        {
            // if value is empty, just return empty string
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            // find the first value that matches the provided value regardless of case
            // if not match is found, return set to null
            var dictionaryValue =
                dictionary.Values.FirstOrDefault(v => v.Equals(value, StringComparison.InvariantCultureIgnoreCase));

            // if value found return value; otherwise return string.empty
            return string.IsNullOrWhiteSpace(dictionaryValue) == false ? dictionaryValue : string.Empty;
        }
    }
}