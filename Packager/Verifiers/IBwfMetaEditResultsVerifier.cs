using System.Collections.Generic;
using Packager.Observers;

namespace Packager.Verifiers
{
    public interface IBwfMetaEditResultsVerifier
    {
        bool Verify(string output, List<string> targetPaths, IObserverCollection observers);
    }
}