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

        public void LogObjectProperties(object instance)
        {
            var builder = new StringBuilder();
            foreach (var property in instance.GetType().GetProperties())
            {
                var name = property.Name.FromCamelCaseToSpaces();
                var value = property.GetValue(instance);
                builder.AppendFormat("{0}: {1}\n", name, value);
            }
            Log(builder.ToString());
        }

        public void LogEngineIssue(Exception exception)
        {
            throw new NotImplementedException();
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