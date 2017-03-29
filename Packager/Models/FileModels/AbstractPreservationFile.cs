using Common.Models;

namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationFile : AbstractFile
    {
      
        protected AbstractPreservationFile(AbstractFile original, string extension) : 
            base(original, FileUsages.PreservationMaster, extension)
        {
        }
    }
}