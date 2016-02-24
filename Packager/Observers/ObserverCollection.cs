using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Exceptions;

namespace Packager.Observers
{
    public class ObserverCollection : HashSet<IObserver>, IObserverCollection
    {
        public ObserverCollection() : base(new ObserverCollectionEqualityComparer())
        {
        }

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

            if (issue is OperationCanceledException)
            {
                issue = new UserCancelledException(issue);
            }

            foreach (var observer in this)
            {
                observer.LogProcessingError(issue, barcode);
            }
        }

        public string BeginProcessingSection(string barcode, string baseMessage, params object[] elements)
        {
            SetBarcode(barcode);
            NotifyOfBeginSection(barcode, baseMessage, elements);
            return barcode;
        }

        public string BeginSection(string baseMessage, params object[] elements)
        {
            var sectionKey = Guid.NewGuid().ToString();
            NotifyOfBeginSection(sectionKey, baseMessage, elements);
            return sectionKey;
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            foreach (var observer in this)
            {
                observer.EndSection(sectionKey, newTitle, collapse);
            }
        }

        private void SetBarcode(string barcode)
        {
            foreach (var observer in GetObjectObservers())
            {
                observer.CurrentBarcode = barcode;
            }
        }
        
        private IEnumerable<ObjectNLogObserver> GetObjectObservers()
        {
            return this.Select(o => o as ObjectNLogObserver).Where(o => o != null);
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

        private void NotifyOfBeginSection(string key, string baseMessage, params object[] elements)
        {
            foreach (var observer in this)
            {
                observer.BeginSection(key, baseMessage, elements);
            }
        }
    }
}