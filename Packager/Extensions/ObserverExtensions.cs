using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Observers;

namespace Packager.Extensions
{
    public static class ObserverExtensions
    {
        public static void Log(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            foreach (var observer in observers)
            {
                observer.Log(baseMessage, elements);
            }
        }

        
        public static void LogError(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            foreach (var observer in observers)
            {
                observer.LogError(baseMessage, elements);
            }
        }

        private static IEnumerable<IViewModelObserver> ViewModelObservers(this IEnumerable<IObserver> observers)
        {
            return observers.Select(o => (o as IViewModelObserver)).Where(o => o != null);
        }

        public static Guid BeginSection(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            var sectionKey = Guid.NewGuid();
            foreach (var observer in observers.ViewModelObservers())
            {
                observer.BeginSection(sectionKey, baseMessage, elements);
            }
            return sectionKey;
        }

        public static void EndSection(this IEnumerable<IObserver> observers, Guid sectionKey)
        {
            foreach (var observer in observers.ViewModelObservers())
            {
                observer.EndSection(sectionKey);
            }
        }

        public static void FlagAsSuccessful(this IEnumerable<IObserver> observers, Guid sectionKey, string newTitle)
        {
            foreach (var observer in observers.ViewModelObservers())
            {
                observer.FlagAsSuccessful(sectionKey, newTitle);
            }
        }
    }
}