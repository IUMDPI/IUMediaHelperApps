using System.Linq;
using Packager.Models;

namespace Packager.Processors
{
    public interface IProcessor
    {
        void ProcessFile(IGrouping<string,FileModel> batchGrouping);
    }
}