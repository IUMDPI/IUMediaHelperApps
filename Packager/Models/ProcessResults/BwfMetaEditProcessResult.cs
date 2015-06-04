using System.Linq;

namespace Packager.Models.ProcessResults
{
    public class BwfMetaEditProcessResult:AbstractProcessResult
    {
        public override bool Succeeded()
        {
            if (!base.Succeeded())
            {
                return false;
            }

            var targetFileName = StartInfo.Arguments.Split(' ').Last();

            var success = string.Format("{0}: is modified", targetFileName).ToLowerInvariant();
            var nothingToDo = string.Format("{0}: nothing to do", targetFileName).ToLowerInvariant();
            
            return StandardOutput.ToLowerInvariant().Contains(success) 
                || StandardOutput.ToLowerInvariant().Contains(nothingToDo);
        }
    }
}
