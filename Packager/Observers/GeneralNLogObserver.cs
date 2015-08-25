using System;
using NLog;
using Packager.Exceptions;

namespace Packager.Observers
{
    public class GeneralNLogObserver : IObserver
    {
        private const string ThisLoggerName = "GeneralFileLogger";

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public GeneralNLogObserver(string logDirectory)
        {
            LogDirectory = logDirectory;
        }

        private string LogDirectory { get; }

        public void Log(string baseMessage, params object[] elements)
        {
            Log(GetLogEvent(baseMessage, elements));
        }

        public void LogProcessingError(Exception issue, string barcode)
        {
            LogEngineError(issue);
        }

        public void LogEngineError(Exception issue)
        {
            if (issue is LoggedException)
            {
                return;
            }

            Log(GetLogEvent(issue.ToString()));
        }

        public void BeginSection(string sectionKey, string baseMessage, params object[] elements)
        {
            Log(baseMessage, elements);
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            // ignore
        }

        public int UniqueIdentifier => 1;

        private static void Log(LogEventInfo logEvent)
        {
            if (string.IsNullOrWhiteSpace(logEvent.Message))
            {
                return;
            }

            Logger.Log(logEvent);
        }

        protected virtual LogEventInfo GetLogEvent(string baseMessage, params object[] elements)
        {
            var eventInfo = LogEventInfo.Create(LogLevel.Info, ThisLoggerName, string.Format(baseMessage, elements));
            eventInfo.Properties["LogDirectoryName"] = LogDirectory;
            return eventInfo;
        }
    }
}