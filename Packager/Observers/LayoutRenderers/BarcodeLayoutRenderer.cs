using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Packager.Observers.LayoutRenderers
{
    [LayoutRenderer("ProcessingDirectoryName")]
    public class BarcodeLayoutRenderer:LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Properties.ContainsKey("Barcode"))
            {
                builder.Append(logEvent.Properties["Barcode"]);
            }
        }
    }
}
