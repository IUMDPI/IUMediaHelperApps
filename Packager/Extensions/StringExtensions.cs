using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Extensions
{
    public static class StringExtensions
    {
        public static string ToQuoted(this string value)
        {
            return string.Format("\"{0}\"", value);
        }

        public static string ToDefaultIfEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? string.Empty 
                : value;
        }
    }
}
