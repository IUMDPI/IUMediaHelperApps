namespace Packager.Extensions
{
    public static class StringExtensions
    {
        public static string ToQuoted(this string value)
        {
            return string.Format("\"{0}\"", value);
        }

        public static string ToDefaultIfEmpty(this string value, string defaultValue = "")
        {
            return string.IsNullOrWhiteSpace(value) 
                ? defaultValue
                : value;
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
    }
}
