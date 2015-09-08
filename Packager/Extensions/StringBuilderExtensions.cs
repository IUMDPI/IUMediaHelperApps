using System.Text;

namespace Packager.Extensions
{
    public static class StringBuilderExtensions
    {
        public static bool HasContent(this StringBuilder builder)
        {
            return builder != null && builder.Length >= 1;
        }
    }
}