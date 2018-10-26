using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public abstract class AbstractFFMPEGArgumentGenerator : IFFMPEGArgumentsGenerator
    {
        protected ArgumentBuilder AccessArguments { get; set; }
        protected ArgumentBuilder NormalizingArguments { get; set; }
        protected ArgumentBuilder ProdOrMezzArguments { get; set; }

        public ArgumentBuilder GetAccessArguments() => AccessArguments;
        public ArgumentBuilder GetNormalizingArguments() => NormalizingArguments;
        public ArgumentBuilder GetProdOrMezzArguments() => ProdOrMezzArguments;
    }
}