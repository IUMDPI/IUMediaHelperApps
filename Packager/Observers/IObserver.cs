using System;

namespace Packager.Observers
{
    public interface IObserver
    {
        void Log(string baseMessage, params object[] elements);
        void LogProcessingError(Exception issue, string barcode);
        void LogEngineError(Exception issue);
    }

    public interface IViewModelObserver : IObserver
    {
        void BeginSection(Guid sectionKey, string baseMessage, params object[] elements);
        void FlagAsSuccessful(Guid sectionKey, string newTitle);
        void FlagAsWarning(Guid sectionKey, string newTitle);
        void FlagAsError(Guid sectionKey, string newTitle);
        void EndSection(Guid sectionKey, string newTitle = "", bool collapse= false);
    }
}