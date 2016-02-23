using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;

namespace Packager.Utilities.ProcessRunners
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }

        string ProdOrMezzArguments { get; }
        string AccessArguments { get; }

        Task<string> GetFFMPEGVersion();

        Task Normalize(AbstractFile original, AbstractEmbeddedMetadata metadata, CancellationToken cancellationToken);

        Task Verify(List<AbstractFile> originals, CancellationToken cancellationToken);

        Task<AbstractFile> CreateProdOrMezzDerivative(AbstractFile original, AbstractFile target, AbstractEmbeddedMetadata metadata, CancellationToken cancellationToken);

        Task<AbstractFile> CreateAccessDerivative(AbstractFile original, CancellationToken cancellationToken);
    }
}