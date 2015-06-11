using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Packager.Providers
{
    internal class AppConfigLookupsProvider : ILookupsProvider
    {
        private const string UnitsPath = "LookupTables/Units";
        public AppConfigLookupsProvider()
        {
            Units = GetDictionary(UnitsPath);
        }

        private static Dictionary<string, string> GetDictionary(string section)
        {
            return ((Hashtable)ConfigurationManager.GetSection(section))
                .Cast<DictionaryEntry>()
                .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
        }

        public Dictionary<string, string> Units { get; private set; }
    }
}