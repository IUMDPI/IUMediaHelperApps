using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Images
{
    public interface ILabelImageImporter
    {
        Task<List<AbstractFile>> ImportMediaImages(string barcode, CancellationToken cancellationToken);
    }
}