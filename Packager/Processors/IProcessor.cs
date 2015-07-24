using System.Linq;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Processors
{
    public interface IProcessor
    {
        Task<bool> ProcessFile(IGrouping<string, AbstractFileModel> fileModels);
    }
}