using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToQuoted(this string value)
        {
            return $"\"{value}\"";
        }

        private static string FromCamelCaseToSpaces(this string value)
        {
            return value.IsNotSet()
                ? value
                : Regex.Replace(value, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static string NormalizeForCommandLine(this string value)
        {
            return value.IsNotSet() 
                ? value 
                : value.Replace("\"","\\\"");
        }

        public static void InsertBefore(this List<string> parts, Predicate<string> predicate, string toInsert = "")
        {
            // find all matching indexes
            var indexes = parts.FindAll(predicate).Select(r => parts.IndexOf(r)).Where(i=>i>=0).ToArray();
            if (indexes.Any() == false)
            {
                return;
            }

            // if only one match found, just insert and return
            if (indexes.Length == 1)
            {
                parts.Insert(indexes.First(), toInsert);
                return;
            }

            // otherwise, seperate matches into blocks
            // a block equals an index that is not one higher
            // than the previous index. This means that the
            // two indexes in question to not follow each 
            // other.
            var blockIndexes = new List<int>();
            for (var i = indexes.Length - 1; i >= 0; i--)
            {
                if (i !=0 && (indexes[i] -1 == indexes[i - 1]))
                {
                    continue;
                }

                blockIndexes.Add(indexes[i]);
            }

            // now insert the lines for each block index
            // note that block index values should be in reverse
            // order, so inserting lines will not affect the indexes
            // of the following indexes 
            foreach (var blockIndex in blockIndexes)
            {
                parts.Insert(blockIndex, toInsert);
            }
            
        }
        
        public static string ToDefaultIfEmpty(this object value, string defaultValue = "")
        {
            if (value == null)
            {
                return defaultValue;
            }

            return value.ToString().IsSet()
                ? value.ToString()
                : defaultValue;
        }

        public static string GetStringPropertiesAndValues(this object instance, string indent = "")
        {
            var builder = new StringBuilder();
            foreach (var property in instance.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string)))
            {
                builder.AppendFormat("{0}{1}: {2}\n",
                    indent,
                    property.Name.FromCamelCaseToSpaces(),
                    property.GetValue(instance).ToDefaultIfEmpty("[not set]"));
            }

            return builder.ToString().TrimEnd('\n');
        }

        public static string GetDatePropertiesAndValues(this object instance, string indent = "")
        {
            var builder = new StringBuilder();
            
            foreach (var property in instance.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime?)))
            {
                builder.AppendFormat("{0}{1}: {2}\n",
                    indent,
                    property.Name.FromCamelCaseToSpaces(),
                    property.GetValue(instance).ToDefaultIfEmpty("[not set]"));
            }

            return builder.ToString().TrimEnd('\n');
        }

        public static string RemoveSpaces(this string value)
        {
            return value.IsNotSet()
                ? value 
                : value.Replace(" ", "");
        }

        public static string TrimWhiteSpace(this string value)
        {
            return value.IsNotSet()
                ? value
                : value.Trim();
        }

        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        public static string AppendIfValuePresent(this string value, string toAppend)
        {
            if (value.IsNotSet() || toAppend.IsNotSet())
            {
                return value;
            }
            
            return value.Trim() + toAppend;
        }

        public static string PrependIfValuePresent(this string value, string toPrepend)
        {
            if (value.IsNotSet() || toPrepend.IsNotSet())
            {
                return value;
            }

            return $"{toPrepend}{value}";
        }

        public static bool IsSet(this string value)
        {
            return string.IsNullOrWhiteSpace(value) == false;
        }

        public static bool IsNotSet(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        
        public static string ToSingularOrPlural<T>(this IEnumerable<T> values, string singular, string plural)
        {
            var count = values?.Count() ?? 0;
            return count.ToSingularOrPlural(singular, plural);
        }

        public static string ToSingularOrPlural<T, TU>(this Dictionary<T, TU> values, string singular, string plural)
        {
            var count = values?.Count ?? 0;
            return count.ToSingularOrPlural(singular, plural);
        }

        public static string ToSingularOrPlural(this int value, string singular, string plural)
        {
            return value == 1
                ? $"{value} {singular}"
                : $"{value} {plural}";
        }

    }
}