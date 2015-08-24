using System;
using System.Collections.Generic;

namespace Packager.Observers
{
    public interface IObserverCollection : ICollection<IObserver>
    {
        void Log(string baseMessage, params object[] elements);
        void LogProcessingIssue(Exception issue, string barcode);
        string BeginProcessingSection(string barcode, string baseMessage, params object[] elements);
        string BeginSection(string baseMessage, params object[] elements);
        void EndSection(string sectionKey, string newTitle = "", bool collapse = false);
        void LogEngineIssue(Exception exception);
    }
}