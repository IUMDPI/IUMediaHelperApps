using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Packager.Exceptions;
using Packager.Extensions;

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

        
        public void LogProcessingIssue(Exception issue, string barcode)
        {
            if (issue is LoggedException)
            {
                return;
            }

            foreach (var observer in this)
            {
                observer.LogProcessingError(issue, barcode);
            }
        }

        public string BeginProcessingSection(string barcode, string baseMessage, params object[] elements)
        {
            NotifyOfBeginSection(barcode, baseMessage, elements);
            return barcode;
        }

        public string BeginSection(string baseMessage, params object[] elements)
        {
            var sectionKey = Guid.NewGuid().ToString();
            NotifyOfBeginSection(sectionKey, baseMessage, elements);
            return sectionKey;
        }

        private void NotifyOfBeginSection(string key, string baseMessage, params object[] elements)
        {
            foreach (var observer in ViewModelObservers())
            {
                observer.BeginSection(key, baseMessage, elements);
            }
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            foreach (var observer in ViewModelObservers())
            {
                observer.EndSection(sectionKey, newTitle, collapse);
            }
        }

        public void LogEngineIssue(Exception exception)
        {
            if (exception is LoggedException)
            {
                return;
            }

            foreach (var observer in this)
            {
                observer.LogEngineError(exception);
            }
        }

        private static string[] GetArrayValues(IEnumerable<object> values)
        {
            if (values == null)
            {
                return new string[0];
            }

            return values.Select(v => v.ToString()).ToArray();
        }
        private IEnumerable<IViewModelObserver> ViewModelObservers()
        {
            return this.Select(o => (o as IViewModelObserver)).Where(o => o != null);
        }
    }
}