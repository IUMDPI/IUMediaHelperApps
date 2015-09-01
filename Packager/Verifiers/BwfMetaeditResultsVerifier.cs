using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Observers;

namespace Packager.Verifiers
{
    public class BwfMetaEditResultsVerifier : IBwfMetaEditResultsVerifier
    {
        public bool Verify(string output, List<string> targetPaths, IObserverCollection observers)
        {
            var hasError = false;
            foreach (var path in targetPaths.Where(path => !IsModifiedOrNothingToDo(output, path)))
            {
                hasError = true;
            }

            return hasError == false;
        }

        public bool Verify(string output, string targetPath, IObserverCollection observers)
        {
            return Verify(output, new List<string> {targetPath}, observers);
        }

        private static bool IsModifiedOrNothingToDo(string output, string path)
        {
            var modified = $"{path}: is modified";
            var nothingToDo = $"{path}: nothing to do";
            return output.Contains(modified) || output.Contains(nothingToDo);
        }
    }
}