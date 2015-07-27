using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Exceptions;

namespace Packager.Observers
{
    public class ObserverCollection : List<IObserver>, IObserverCollection
    {
        public void Log(string baseMessage, params object[] elements)
        {
            foreach (var observer in this)
            {
                observer.Log(baseMessage, elements);
            }
        }

        public void LogIssue(Exception issue)
        {
            if (issue is LoggedException)
            {
                return;
            }

            foreach (var observer in this)
            {
                observer.LogError(issue);
            }
        }

        public Guid BeginSection(string baseMessage, params object[] elements)
        {
            var sectionKey = Guid.NewGuid();
            foreach (var observer in ViewModelObservers())
            {
                observer.BeginSection(sectionKey, baseMessage, elements);
            }
            return sectionKey;
        }

        public void EndSection(Guid sectionKey, string newTitle = "", bool collapse = false)
        {
            foreach (var observer in ViewModelObservers())
            {
                observer.EndSection(sectionKey, newTitle, collapse);
            }
        }

        private IEnumerable<IViewModelObserver> ViewModelObservers()
        {
            return this.Select(o => (o as IViewModelObserver)).Where(o => o != null);
        }
    }
}