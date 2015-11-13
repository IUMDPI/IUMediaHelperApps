using System.Collections.Generic;

namespace Packager.Providers
{
    public interface ILookupsProvider
    {
        Dictionary<string, string> PlaybackSpeed { get; }
        Dictionary<string, string> TrackConfiguration { get; }
        Dictionary<string, string> SoundField { get; }
        Dictionary<string, string> TapeThickness { get; }

        Dictionary<string, string> PreservationProblem { get; set; }
        Dictionary<string, string> Damage { get; set; }

    }
}