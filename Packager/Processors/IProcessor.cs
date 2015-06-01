using System.Linq;
using Packager.Models;
using Packager.Models.FileModels;

namespace Packager.Processors
{
    public interface IProcessor
    {
        void ProcessFile(IGrouping<string,AbstractFileModel> batchGrouping);
    }
}