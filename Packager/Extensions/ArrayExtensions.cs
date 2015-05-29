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
    }
}
