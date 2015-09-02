using System.Collections.Generic;
using Packager.Observers;

namespace Packager.Verifiers
{
    public interface IBwfMetaEditResultsVerifier
    {
        bool Verify(string output, IEnumerable<string> targetPaths);
        bool Verify(string output, string targetPath);
    }
}
