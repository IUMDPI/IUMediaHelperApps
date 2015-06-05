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

        

        public static int? ToInteger(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int result;
            if (!int.TryParse(value, out result))
            {
                return null;
            }

            return result;
        }
    }
}
