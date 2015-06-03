using System.Linq;
using System.Threading.Tasks;
using Packager.Models;
using Packager.Models.FileModels;

namespace Packager.Processors
{
    public interface IProcessor
    {
        Task ProcessFile(IGrouping<string,AbstractFileModel> barcodeGrouping);
    }
}