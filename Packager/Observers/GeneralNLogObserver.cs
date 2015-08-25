using System;
using System.Collections.Generic;
using System.Linq;
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
            Log(GetLogEvents(baseMessage, elements));
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

            Log(GetLogEvents(issue.ToString()));
        }

        public void BeginSection(string sectionKey, string baseMessage, params object[] elements)
        {
            Log("----------------------------------------");
            Log(baseMessage, elements);
            Log("----------------------------------------");
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            Log(new[] {GetLogEvent(" ")});
        }

        public int UniqueIdentifier => 1;

        private static void Log(IEnumerable<LogEventInfo> events)
        {
            foreach (var logEvent in events.Where(e=>!string.IsNullOrWhiteSpace(e.Message)))
            {
                Logger.Log(logEvent);
            }
        }

        private static string NormalizeMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            message = message.Replace("\n", "\r\n");
            message = message.Replace("\t", "");
            message = message.Trim();
            return message;
        }

        private LogEventInfo[] GetLogEvents(string baseMessage, params object[] elements)
        {
            var message = NormalizeMessage(string.Format(baseMessage, elements));

            return message.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(GetLogEvent).ToArray();
        }

        private LogEventInfo GetLogEvent(string message)
        {
            var eventInfo = LogEventInfo.Create(LogLevel.Info, ThisLoggerName, message);
            eventInfo.Properties["LogDirectoryName"] = LogDirectory;
            return eventInfo;
        }
    }
}