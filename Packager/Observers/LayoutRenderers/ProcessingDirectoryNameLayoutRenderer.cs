using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Packager.Observers.LayoutRenderers
{
    [LayoutRenderer("ProcessingDirectoryName")]
    public class ProcessingDirectoryNameLayoutRenderer:LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.Properties["ProcessingDirectoryName"]);
        }
    }
}
