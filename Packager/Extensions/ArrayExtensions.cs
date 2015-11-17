using System.Collections.Generic;
using System.Linq;

namespace Packager.Extensions
{
    public static class ArrayExtensions
    {
        public static T FromIndex<T>(this T[] parts, int index, T defaultValue)
        {
            return parts.Length >= index + 1 ? parts[index] : defaultValue;
        }

        public static string ToSingularOrPlural<T>(this IEnumerable<T> values, string singular, string plural)
        {
            var count = values?.Count() ?? 0;
            return count.ToSingularOrPlural(singular, plural);
        }

        public static string ToSingularOrPlural<T, TU>(this Dictionary<T, TU> values, string singular, string plural)
        {
            var count = values?.Count() ?? 0;
            return count.ToSingularOrPlural(singular, plural);
        }

        public static string ToSingularOrPlural(this int value, string singular, string plural)
        {
            return value == 1
                ? $"{value} {singular}"
                : $"{value} {plural}";
        }

        public static bool IsFirst<T>(this IEnumerable<T> list, T instance)
        {
            return list.First().Equals(instance);
        }
    }
}