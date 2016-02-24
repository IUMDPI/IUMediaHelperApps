using System.Collections.Generic;
using System.Linq;
using NLog;
using Packager.Extensions;
using Packager.Models.SettingsModels;

namespace Packager.Observers
{
    public class GeneralNLogObserver : AbstractNLogObserver
    {
        private const string ThisLoggerName = "GeneralFileLogger";

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public GeneralNLogObserver(IProgramSettings settings):base(ThisLoggerName, settings)
        {
        }
        
        protected override void Log(IEnumerable<LogEventInfo> events)
        {
            foreach (var logEvent in events.Where(e => e.Message.IsSet()))
            {
                Logger.Log(logEvent);
            }
        }

        protected override void LogEvent(LogEventInfo eventInfo)
        {
            Logger.Log(eventInfo);
        }

        public override int UniqueIdentifier => 1;
    }
}