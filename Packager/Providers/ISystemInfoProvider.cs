namespace Packager.Providers
{
    public interface ISystemInfoProvider
    {
        string MachineName { get; }
        string CurrentSystemLogPath { get; }
    }
}