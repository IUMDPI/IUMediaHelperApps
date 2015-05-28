namespace Packager.Observers
{
    public interface IObserver
    {
        void Log(string baseMessage, params object[] elements);
        void LogHeader(string baseMessage, params object[] elements);
    }
}