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
            if (logEvent.Properties.ContainsKey("LogDirectoryName"))
            {
                builder.Append(logEvent.Properties["LogDirectoryName"]);
            }
        }
    }
}
