using System.Linq;
using Packager.Models;

namespace Packager.Utilities
{
    internal interface IProcessor
    {
        void ProcessFile(IGrouping<string,FileModel> batchGrouping);
    }
}