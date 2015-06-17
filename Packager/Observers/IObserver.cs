using System;

namespace Packager.Observers
{
    public interface IObserver
    {
        void Log(string baseMessage, params object[] elements);
        void LogHeader(string baseMessage, params object[] elements);
        void LogError(string baseMessage, object[] elements);
        void LogExternal(string text);

        void BeginSection(Guid sectionKey, string baseMessage, params object[] elements);
        void EndSection(Guid sectionKey);
    }
}