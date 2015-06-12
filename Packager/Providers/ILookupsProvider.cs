using System.Collections.Generic;

namespace Packager.Providers
{
    public enum LookupTables
    {
        Units,
        PlaybackSpeeds,
        TrackConfigurations,
        SoundFields,
        TapeThicknesses
    }
    
    public interface ILookupsProvider
    {
        Dictionary<string, string> Units { get; }
        Dictionary<string, string> PlaybackSpeeds { get; }
        Dictionary<string, string> TrackConfigurations { get; }
        Dictionary<string, string> SoundFields { get; }
        Dictionary<string, string> TapeThicknesses { get; }

        string LookupValue(LookupTables table, string value, string defaultValue="");
        string[] LookupValue(LookupTables table, string[] values);
    }
}