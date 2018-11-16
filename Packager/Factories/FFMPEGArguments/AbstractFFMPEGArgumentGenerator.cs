using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public abstract class AbstractFFMPEGArgumentGenerator : IFFMPEGArgumentsGenerator
    {
        protected ArgumentBuilder AccessArguments { get; set; }
        protected ArgumentBuilder NormalizingArguments { get; set; }
        protected ArgumentBuilder ProdOrMezzArguments { get; set; }

        public ArgumentBuilder GetAccessArguments() => AccessArguments.Clone();
        public ArgumentBuilder GetNormalizingArguments() => NormalizingArguments.Clone();
        public ArgumentBuilder GetProdOrMezzArguments() => ProdOrMezzArguments.Clone();
    }
}