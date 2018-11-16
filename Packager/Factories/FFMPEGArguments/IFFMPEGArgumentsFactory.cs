using Common.Models;
using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public interface IFFMPEGArgumentsFactory
    {
        ArgumentBuilder GetAccessArguments(IMediaFormat format);
        ArgumentBuilder GetNormalizingArguments(IMediaFormat format);
        ArgumentBuilder GetProdOrMezzArguments(IMediaFormat format);
    }
}
