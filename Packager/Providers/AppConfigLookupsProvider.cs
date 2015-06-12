using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Packager.Providers
{
    internal class AppConfigLookupsProvider : ILookupsProvider
    {
        private const string UnitsPath = "LookupTables/Units";
        private const string PlaybackSpeedsPath = "LookupTables/PlaybackSpeeds";
        private const string TrackConfigurationsPath = "LookupTables/TrackConfigurations";
        private const string SoundFieldsPath = "LookupTables/SoundFields";
        private const string ThicknessesPath = "LookupTables/TapeThicknesses";

        public AppConfigLookupsProvider()
        {
            Units = GetDictionary(UnitsPath);
            PlaybackSpeeds = GetDictionary(PlaybackSpeedsPath);
            TrackConfigurations = GetDictionary(TrackConfigurationsPath);
            SoundFields = GetDictionary(SoundFieldsPath);
            TapeThicknesses = GetDictionary(ThicknessesPath);
        }

        public Dictionary<string, string> Units { get; private set; }
        public Dictionary<string, string> PlaybackSpeeds { get; private set; }
        public Dictionary<string, string> TrackConfigurations { get; private set; }
        public Dictionary<string, string> SoundFields { get; private set; }
        public Dictionary<string, string> TapeThicknesses { get; private set; }

        public string LookupValue(LookupTables table, string value, string defaultValue = null)
        {
            switch (table)
            {
                case LookupTables.Units:
                    return GetValue(Units, value, defaultValue);
                case LookupTables.PlaybackSpeeds:
                    return GetValue(PlaybackSpeeds, value, defaultValue);
                case LookupTables.SoundFields:
                    return GetValue(SoundFields, value, defaultValue);
                case LookupTables.TrackConfigurations:
                    return GetValue(TrackConfigurations, value, defaultValue);
                case LookupTables.TapeThicknesses:
                    return GetValue(TapeThicknesses, value, defaultValue);
            }

            return defaultValue;
        }

        public string[] LookupValue(LookupTables table, string[] values)
        {
            return values.Select(value => LookupValue(table, value)).ToArray();
        }

        private static Dictionary<string, string> GetDictionary(string section)
        {
            var hashTable = ConfigurationManager.GetSection(section) as Hashtable;
            if (hashTable == null)
            {
                return new Dictionary<string, string>();
            }
            
            return hashTable
                .Cast<DictionaryEntry>()
                .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
        }

        private static string GetValue(IReadOnlyDictionary<string, string> table, string key, string defaultValue =null)
        {
            return table.ContainsKey(key)
                ? table[key]
                : defaultValue;
        }
    }
}