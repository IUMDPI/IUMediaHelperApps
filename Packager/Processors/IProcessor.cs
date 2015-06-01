using System.Linq;
using Packager.Models;

namespace Packager.Processors
{
    internal interface IProcessor
    {
        void ProcessFile(IGrouping<string,FileModel> batchGrouping);
    }
}