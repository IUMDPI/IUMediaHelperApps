using System;
using System.Collections.Specialized;
using System.Linq;
using Packager.Extensions;

namespace Packager.Factories
{
    public interface ISettingsFactory
    {
        string GetStringValue(NameValueCollection dictionary, string name);
        int GetIntValue(NameValueCollection dictionary, string name, int defaultValue);
        string[] GetStringValues(NameValueCollection dictionary, string name);
    }

    public class SettingsFactory : ISettingsFactory
    {
        public string GetStringValue(NameValueCollection dictionary, string name)
        {
            return dictionary[name].ToDefaultIfEmpty();
        }

        public int GetIntValue(NameValueCollection dictionary, string name, int defaultValue)
        {
            int result;
            return int.TryParse(GetStringValue(dictionary, name), out result)
                ? result
                : defaultValue;
        }

        public string[] GetStringValues(NameValueCollection dictionary, string name)
        {
            return GetStringValue(dictionary, name)
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim()) // trim initial and trailing spaces
                .Where(e => string.IsNullOrWhiteSpace(e) == false) // remove entries that are empty (just in case)
                .ToArray();
        }
    }
}