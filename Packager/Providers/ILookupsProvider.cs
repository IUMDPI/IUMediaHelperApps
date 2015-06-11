using System.Collections.Generic;

namespace Packager.Providers
{
    public interface ILookupsProvider
    {
        Dictionary<string, string> Units { get; }
    }
}