using System;
using NLog;

namespace Packager.Observers
{
    public class GeneralNLogObserver : AbstractNLogObserver
    {
        private const string ThisLoggerName = "GeneralFileLogger";

        public GeneralNLogObserver(string logDirectory, string processingDirectory)
            : base(logDirectory, processingDirectory)
        {
        }

        protected override string LoggerName => ThisLoggerName;

        public override void Log(string baseMessage, params object[] elements)
        {
            Logger.Log(GetLogEvent(baseMessage, elements));
        }

        public override void LogProcessingError(Exception issue, string barcode)
        {
            LogEngineError(issue);
        }

        public override int UniqueIdentifier => 1;

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);
    }
}