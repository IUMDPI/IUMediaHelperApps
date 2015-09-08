using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Observers;

namespace Packager.Verifiers
{
    public class BwfMetaEditResultsVerifier : IBwfMetaEditResultsVerifier
    {
        public bool Verify(string output, IEnumerable<string> targetPaths)
        {
            var hasError = false;
            foreach (var path in targetPaths.Where(path => !IsModifiedOrNothingToDo(output, path)))
            {
                hasError = true;
            }

            return hasError == false;
        }

        public bool Verify(string output, string targetPath)
        {
            return Verify(output, new List<string> {targetPath});
        }

        private static bool IsModifiedOrNothingToDo(string output, string path)
        {
            var modified = $"{path.ToLowerInvariant()}: is modified";
            var nothingToDo = $"{path.ToLowerInvariant()}: nothing to do";
            return output.Contains(modified) || output.Contains(nothingToDo);
        }
    }
}