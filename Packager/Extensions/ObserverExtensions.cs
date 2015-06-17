using System;
using System.Collections.Generic;
using NLog.Targets;
using Packager.Models;
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

        public static void LogHeader(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            foreach (var observer in observers)
            {
                observer.LogHeader(baseMessage, elements);
            }
        }

        public static void LogError(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            foreach (var observer in observers)
            {
                observer.LogError(baseMessage, elements);
            }
        }

        public static void LogExternal(this IEnumerable<IObserver> observers, string text)
        {
            foreach (var observer in observers)
            {
                observer.LogExternal(text);
            }
        }

        public static Guid BeginSection(this IEnumerable<IObserver> observers, string baseMessage, params object[] elements)
        {
            var sectionKey = Guid.NewGuid();
            foreach (var observer in observers)
            {
                observer.BeginSection(sectionKey, baseMessage, elements);
            }
            return sectionKey;
        }

        public static void EndSection(this IEnumerable<IObserver> observers, Guid sectionKey)
        {
            foreach (var observer in observers)
            {
                observer.EndSection(sectionKey);
            }
        }
    }
}