using System.Linq;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Validators;

namespace Packager.Processors
{
    public interface IProcessor
    {
        Task<ValidationResult> ProcessFile(IGrouping<string, AbstractFile> fileModels);
    }
}