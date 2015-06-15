using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Packager.Exceptions;

namespace Packager.Providers
{
    internal class AppConfigLookupsProvider : ILookupsProvider
    {
        private const string UnitsPath = "LookupTables/Units";
        private const string PlaybackSpeedsPath = "LookupTables/PlaybackSpeeds";
        private const string TrackConfigurationsPath = "LookupTables/TrackConfigurations";
        private const string SoundFieldsPath = "LookupTables/SoundFields";
        private const string ThicknessesPath = "LookupTables/TapeThicknesses";

        private readonly Dictionary<LookupTables, Dictionary<string, string>> _tables;

        public AppConfigLookupsProvider()
        {
            Units = GetDictionary(UnitsPath);
            PlaybackSpeeds = GetDictionary(PlaybackSpeedsPath);
            TrackConfigurations = GetDictionary(TrackConfigurationsPath);
            SoundFields = GetDictionary(SoundFieldsPath);
            TapeThicknesses = GetDictionary(ThicknessesPath);

            _tables = new Dictionary<LookupTables, Dictionary<string, string>>
            {
                {LookupTables.Units, Units},
                {LookupTables.PlaybackSpeeds, PlaybackSpeeds},
                {LookupTables.SoundFields, SoundFields},
                {LookupTables.TrackConfigurations, TrackConfigurations},
                {LookupTables.TapeThicknesses, TapeThicknesses}
            };
        }

        public Dictionary<string, string> Units { get; private set; }
        public Dictionary<string, string> PlaybackSpeeds { get; private set; }
        public Dictionary<string, string> TrackConfigurations { get; private set; }
        public Dictionary<string, string> SoundFields { get; private set; }
        public Dictionary<string, string> TapeThicknesses { get; private set; }

        public string LookupValue(LookupTables table, string value)
        {
            if (!_tables.ContainsKey(table))
            {
                throw new LookupException("No lookup handler for table {0}", table);    
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var result = GetValue(_tables[table], value);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new LookupException("No value present for key {0} in lookup table {1}", value, table);
            }

            return result;

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

        private static string GetValue(IReadOnlyDictionary<string, string> table, string key)
        {
            return table.ContainsKey(key)
                ? table[key]
                : null;
        }
    }
}