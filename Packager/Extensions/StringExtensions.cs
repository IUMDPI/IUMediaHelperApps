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

        public static string FromCamelCaseToSpaces(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Regex.Replace(value, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
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
    }
}