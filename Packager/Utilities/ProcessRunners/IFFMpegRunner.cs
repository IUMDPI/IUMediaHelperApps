using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.ProcessRunners
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        
        Task<string> GetFFMPEGVersion();

        Task Normalize(AbstractFile original, ArgumentBuilder arguments, CancellationToken cancellationToken);

        Task Verify(List<AbstractFile> originals, CancellationToken cancellationToken);

        Task<AbstractFile> CreateProdOrMezzDerivative(AbstractFile original, AbstractFile target, ArgumentBuilder arguments, IEnumerable<string> notes, CancellationToken cancellationToken);

        Task<AbstractFile> CreateAccessDerivative(AbstractFile original, ArgumentBuilder arguments, IEnumerable<string> notes, CancellationToken cancellationToken);
    }
}