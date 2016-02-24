using NLog;
using Packager.Extensions;
using Packager.Models.SettingsModels;

namespace Packager.Observers
{
    public class ObjectNLogObserver : AbstractNLogObserver
    {
        private const string ThisLoggerName = "ObjectFileLogger";

        public string CurrentBarcode { private get; set; }
        
        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public ObjectNLogObserver(IProgramSettings settings) : base(ThisLoggerName, settings)
        {
            
        }

        public void ClearBarcode()
        {
            CurrentBarcode = string.Empty;
        }

        protected override LogEventInfo GetLogEvent(string message)
        {
            var logEvent = base.GetLogEvent(message);
            logEvent.Properties["Barcode"] = CurrentBarcode;
            return logEvent;
        }

        protected override void LogEvent(LogEventInfo eventInfo)
        {
            if (CurrentBarcode.IsNotSet())
            {
                return;
            }

            Logger.Log(eventInfo);
        }
        
        public override int UniqueIdentifier => 2;
    }
}