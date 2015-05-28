using System.Collections.Generic;
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
    }
}