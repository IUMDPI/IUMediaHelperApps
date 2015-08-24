using System;

namespace Packager.Observers
{
    public interface IObserver
    {
        void Log(string baseMessage, params object[] elements);
        void LogProcessingError(Exception issue, string barcode);
        void LogEngineError(Exception issue);

        int UniqueIdentifier { get; }
    }

    public interface IViewModelObserver : IObserver
    {
        void BeginSection(string sectionKey, string baseMessage, params object[] elements);
      
        void EndSection(string sectionKey, string newTitle = "", bool collapse= false);
    }
}