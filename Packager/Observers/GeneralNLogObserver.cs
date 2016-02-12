using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Packager.Exceptions;
using Packager.Extensions;

namespace Packager.Observers
{

    public abstract class AbstractNLogObserver : IObserver
    {
        private readonly string _loggerName;
        private readonly string _logDirectory;

        protected AbstractNLogObserver(string loggerName, string logDirectory)
        {
            _loggerName = loggerName;
            _logDirectory = logDirectory;
        }

        protected abstract void LogEvent(LogEventInfo eventInfo);

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

        public abstract int UniqueIdentifier { get; }

        private IEnumerable<LogEventInfo> GetLogEvents(string baseMessage, params object[] elements)
        {
            var message = NormalizeMessage(string.Format(baseMessage, elements));

            return message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(GetLogEvent).ToArray();
        }

        protected virtual LogEventInfo GetLogEvent(string message)
        {
            var eventInfo = LogEventInfo.Create(LogLevel.Info, _loggerName, message);
            eventInfo.Properties["LogDirectoryName"] = _logDirectory;
            return eventInfo;
        }
        
        private static string NormalizeMessage(string message)
        {
            if (message.IsNotSet())
            {
                return message;
            }

            message = message.Replace("\n", "\r\n");
            message = message.Replace("\t", "");
            message = message.Trim();
            return message;
        }

        protected virtual void Log(IEnumerable<LogEventInfo> events)
        {
            foreach (var logEvent in events.Where(e => e.Message.IsSet()))
            {
                LogEvent(logEvent);
            }
        }
    }

    public class GeneralNLogObserver : AbstractNLogObserver
    {
        private const string ThisLoggerName = "GeneralFileLogger";

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public GeneralNLogObserver(string logDirectory):base(ThisLoggerName, logDirectory)
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

    public class ObjectNLogObserver : AbstractNLogObserver
    {
        public string CurrentBarcode { get; set; }
        private const string ThisLoggerName = "ObjectFileLogger";

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);

        public ObjectNLogObserver(string logDirectory) : base(ThisLoggerName, logDirectory)
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