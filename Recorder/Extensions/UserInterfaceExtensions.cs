using System.Windows;
using System.Windows.Media;

namespace Recorder.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T FindDescendant<T>(this DependencyObject target) where T : DependencyObject
        {
            if (target == null) return default(T);
            var numberChildren = VisualTreeHelper.GetChildrenCount(target);
            if (numberChildren == 0) return default(T);

            for (var i = 0; i < numberChildren; i++)
            {
                var child = VisualTreeHelper.GetChild(target, i);
                var instance = child as T;
                if (instance != null)
                {
                    return instance;
                }
            }

            for (var i = 0; i < numberChildren; i++)
            {
                var child = VisualTreeHelper.GetChild(target, i);
                var potentialMatch = FindDescendant<T>(child);
                if (potentialMatch.Equals(default(T)) == false)
                {
                    return potentialMatch;
                }
            }

            return default(T);
        }
    }
}