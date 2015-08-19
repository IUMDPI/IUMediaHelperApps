using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (values == null || values.Count() == 1)
            {
                return singular;
            }
            
            return plural;
        }

        public static string ToSingularOrPlural<T, TU>(this Dictionary<T, TU> values, string singular, string plural)
        {
            if (values == null || values.Count() == 1)
            {
                return singular;
            }

            return plural;
        }
    }
}
