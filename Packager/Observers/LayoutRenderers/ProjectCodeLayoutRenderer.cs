using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Packager.Observers.LayoutRenderers
{
    [LayoutRenderer("ProcessingDirectoryName")]
    public class ProjectCodeLayoutRenderer:LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Properties.ContainsKey("ProjectCode"))
            {
                builder.Append(logEvent.Properties["ProjectCode"]);
            }
        }
    }
}
