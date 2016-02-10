using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;

namespace Packager.Utilities.Process
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }

        string ProdOrMezzArguments { get; }
        string AccessArguments { get; }

        Task<string> GetFFMPEGVersion();

        Task Normalize(AbstractFile original, AbstractEmbeddedMetadata metadata);

        Task Verify(List<AbstractFile> originals);

        Task<AbstractFile> CreateProdOrMezzDerivative(AbstractFile original, AbstractFile target, AbstractEmbeddedMetadata metadata);

        Task<AbstractFile> CreateAccessDerivative(AbstractFile original);
    }
}