using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Packager.Validators.Attributes;

namespace Packager.Providers
{
    internal class AppConfigLookupsProvider : ILookupsProvider
    {
        
        private const string PlaybackSpeedsPath = "LookupTables/PlaybackSpeed";
        private const string TrackConfigurationsPath = "LookupTables/TrackConfiguration";
        private const string SoundFieldsPath = "LookupTables/SoundField";
        private const string ThicknessesPath = "LookupTables/TapeThickness";
        private const string PreservationProblemPath = "LookupTables/PreservationProblem";
        private const string DamagePath = "LookupTables/Damage";

        public AppConfigLookupsProvider()
        {
            PlaybackSpeed = InitializeDictionary(PlaybackSpeedsPath);
            TrackConfiguration = InitializeDictionary(TrackConfigurationsPath);
            SoundField = InitializeDictionary(SoundFieldsPath);
            TapeThickness = InitializeDictionary(ThicknessesPath);
            PreservationProblem = InitializeDictionary(PreservationProblemPath);
            Damage = InitializeDictionary(DamagePath);
        }

        [HasMembers]
        public Dictionary<string, string> PlaybackSpeed { get; }

        [HasMembers]
        public Dictionary<string, string> TrackConfiguration { get; }

        [HasMembers]
        public Dictionary<string, string> SoundField { get; }

        [HasMembers]
        public Dictionary<string, string> TapeThickness { get; }

        [HasMembers]
        public Dictionary<string, string> PreservationProblem { get; set; }

        [HasMembers]
        public Dictionary<string, string> Damage { get; set; }

        private static Dictionary<string, string> InitializeDictionary(string section)
        {
           
            var hashTable = ConfigurationManager.GetSection(section) as Hashtable;
            if (hashTable == null)
            {
                return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            }

            // convert hashtable into normal dictionary
            // and then create a case-insensitive dictionary (as far as keys are concerned)
            // from it
            return new Dictionary<string, string>(
                hashTable.Cast<DictionaryEntry>().ToDictionary(n => n.Key.ToString(), n => n.Value.ToString()),
                StringComparer.InvariantCultureIgnoreCase);
           
        }
    }
}