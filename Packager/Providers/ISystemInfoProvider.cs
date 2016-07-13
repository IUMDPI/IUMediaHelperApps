using Packager.Engine;

namespace Packager.Providers
{
    public interface ISystemInfoProvider
    {
        string MachineName { get; }
        string CurrentSystemLogPath { get; }
        void ExitApplication(EngineExitCodes exitCode);
    }
}