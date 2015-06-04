using System.Threading.Tasks;
using NSubstitute;
using Packager.Models.ProcessResults;
using Packager.Utilities;

namespace Packager.Test.Mocks
{
    public static class MockProcessRunner
    {
        public static IProcessRunner Get(IProcessResult bwfMetaEditResult = null, IProcessResult ffmpegResult = null)
        {
            var runner = Substitute.For<IProcessRunner>();

            if (bwfMetaEditResult == null)
            {
                bwfMetaEditResult = Substitute.For<IProcessResult>();
                bwfMetaEditResult.Succeeded().ReturnsForAnyArgs(true);
            }

            if (ffmpegResult == null)
            {
                ffmpegResult = Substitute.For<IProcessResult>();
                ffmpegResult.Succeeded().ReturnsForAnyArgs(true);
            }

            runner.Run<BwfMetaEditProcessResult>(null).ReturnsForAnyArgs(Task.FromResult(bwfMetaEditResult));
            runner.Run<FFMPEGProcessResult>(null).ReturnsForAnyArgs(Task.FromResult(ffmpegResult));
            return runner;
        }
    }
}