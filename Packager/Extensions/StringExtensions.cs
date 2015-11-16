using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Packager.Extensions
{
    public static class StringExtensions
    {
        public static string ToQuoted(this string value)
        {
            return string.Format("\"{0}\"", value);
        }

        public static int? ToInteger(this string value, int? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            int result;
            if (!int.TryParse(value, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static bool? ToBool(this string value, bool? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            bool result;
            if (!bool.TryParse(value, out result))
            {
                return defaultValue;
            }

            return result;
        }

        private static string FromCamelCaseToSpaces(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Regex.Replace(value, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static string NormalizeForCommandLine(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return value.Replace("\"","\\\"");

        }

        public static string ToDefaultIfEmpty(this object value, string defaultValue = "")
        {
            if (value == null)
            {
                return defaultValue;
            }

            return string.IsNullOrWhiteSpace(value.ToString()) == false
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

            return builder.ToString();
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

            return builder.ToString();
        }

        public static string RemoveSpaces(this string value)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? value 
                : value.Replace(" ", "");
        }

        public static string TrimWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? value
                : value.Trim();
        }

        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}