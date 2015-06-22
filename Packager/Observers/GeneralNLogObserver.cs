using System;
using NLog;

namespace Packager.Observers
{
    public class GeneralNLogObserver : AbstractNLogObserver
    {
        private const string ThisLoggerName = "GeneralFileLogger";

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public GeneralNLogObserver(string logDirectory, string processingDirectory)
            : base(logDirectory, processingDirectory)
        {
        }
        
        public override void Log(string baseMessage, params object[] elements)
        {
            Logger.Log(GetLogEvent(baseMessage, elements));
        }

        public override void LogHeader(string baseMessage, params object[] elements)
        {
            Log(baseMessage, elements);
        }

        public override void LogError(Exception issue)
        {
            Log(issue.ToString());
        }

        protected override string LoggerName { get {return ThisLoggerName;} }
    }
}