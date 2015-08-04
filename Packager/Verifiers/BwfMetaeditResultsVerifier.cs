using System.Collections.Generic;
using System.IO;
using Packager.Observers;

namespace Packager.Verifiers
{
    public interface IBwfMetaEditResultsVerifier
    {
        bool Verify(string output, List<string> targetPaths, IObserverCollection observers);
    }

    public class BwfMetaEditResultsVerifier : IBwfMetaEditResultsVerifier
    {
        public bool Verify(string output, List<string> targetPaths, IObserverCollection observers)
        {
            var hasError = false;
            foreach (var path in targetPaths)
            {
                var fileName = Path.GetFileName(path);

                if (IsModifiedOrNothingToDo(output, path)) continue; // no error continue

                observers.Log("Could not add metadata to {0}", fileName);
                hasError = true;
            }

            return hasError == false;
        }

        private static bool IsModifiedOrNothingToDo(string output, string path)
        {
            var modified = string.Format("{0}: is modified", path);
            var nothingToDo = string.Format("{0}: nothing to do", path);
            return output.Contains(modified) || output.Contains(nothingToDo);
        }
    }
}