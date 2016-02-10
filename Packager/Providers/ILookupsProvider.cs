using System.Collections.Generic;

namespace Packager.Providers
{
    public interface ILookupsProvider
    {
        IDictionary<string, string> PlaybackSpeed { get; }
        IDictionary<string, string> TrackConfiguration { get; }
        IDictionary<string, string> SoundField { get; }
        IDictionary<string, string> TapeThickness { get; }

        IDictionary<string, string> PreservationProblem { get; set; }
        IDictionary<string, string> Damage { get; set; }

    }
}