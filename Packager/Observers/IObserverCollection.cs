﻿using System;
using System.Collections.Generic;

namespace Packager.Observers
{
    public interface IObserverCollection : IList<IObserver>
    {
        void Log(string baseMessage, params object[] elements);
        void LogProcessingIssue(Exception issue, string barcode);
        Guid BeginSection(string baseMessage, params object[] elements);
        void EndSection(Guid sectionKey, string newTitle = "", bool collapse = false);
        void LogEngineIssue(Exception exception);
    }
}