using System.Collections.Generic;
using System.Linq;

namespace Packager.Verifiers
{
    public class BwfMetaEditResultsVerifier : IBwfMetaEditResultsVerifier
    {
        private const string InvalidWavErrorText = "invalid wave:";
        private const string CanceledErrorText = "canceled";
        private const string ErrorDuringReadingText = "error during reading";
        private const string ErrorDuringWritingText = "error during writing";

        public bool Verify(string output, IEnumerable<string> targetPaths)
        {
            return targetPaths.Any(path => !IsModifiedOrNothingToDo(output, path) || HasErrorText(output)) == false;
        }

        public bool Verify(string output, string targetPath)
        {
            return Verify(output, new List<string> {targetPath});
        }

        private static bool HasErrorText(string output)
        {
            return output.Contains(InvalidWavErrorText) ||
                   output.Contains(CanceledErrorText) ||
                   output.Contains(ErrorDuringReadingText) ||
                   output.Contains(ErrorDuringWritingText);
        }

        private static bool IsModifiedOrNothingToDo(string output, string path)
        {
            var modified = $"{path.ToLowerInvariant()}: is modified";
            var nothingToDo = $"{path.ToLowerInvariant()}: nothing to do";
            return output.Contains(modified) || output.Contains(nothingToDo);
        }
    }
}