using System.Linq;
using Packager.Models;

namespace Packager.Utilities
{
    public class SkippingProcessor : IProcessor
    {
        public void ProcessFile(string targetPath)
        {
        }

        public void ProcessFile(IGrouping<string, FileModel> batchGrouping)
        {
            
        }
    }
}