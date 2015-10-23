using System.Text;

namespace Packager.Kludges
{
    // There is an issue with AudioInspector and BatchInspector
    // where either program will crash if the size of the BEXT chunk
    // is reported as uneven.
    // The kludge adds a padding space to coding history
    // if this will force the size of the BEXT chunk to be reported as even
    public static class CodingHistoryKludge
    {
        // For FFMpeg, if the length is even
        // Add a space
        public static void KludgeForFFMpeg(StringBuilder builder)
        {
            if (builder.Length%2 == 0)
            {
                builder.Append(" ");
            }
        }
    }
}