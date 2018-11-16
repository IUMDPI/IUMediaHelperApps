using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public interface IFFMPEGArgumentsGenerator
    {
        ArgumentBuilder GetAccessArguments();
        ArgumentBuilder GetNormalizingArguments();
        ArgumentBuilder GetProdOrMezzArguments();
    }
}