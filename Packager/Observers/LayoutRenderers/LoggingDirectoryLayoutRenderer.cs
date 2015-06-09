using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Packager.Observers.LayoutRenderers
{
    [LayoutRenderer("LogDirectoryName")]
    public class LoggingDirectoryLayoutRenderer:LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.Properties["LogDirectoryName"]);
        }
    }
}
