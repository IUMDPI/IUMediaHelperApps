using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Packager.Observers.LayoutRenderers
{
    [LayoutRenderer("ProcessingDirectoryName")]
    public class ProcessingDirectoryNameLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Properties.ContainsKey("ProcessingDirectoryName"))
            {
                builder.Append(logEvent.Properties["ProcessingDirectoryName"]);
            }
        }
    }
}