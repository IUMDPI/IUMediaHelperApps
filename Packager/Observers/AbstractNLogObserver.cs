using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.SettingsModels;

namespace Packager.Observers
{
    public abstract class AbstractNLogObserver : IObserver
    {
        private readonly string _loggerName;
        private readonly IProgramSettings _settings;
        private string LogDirectory =>_settings.LogDirectoryName;
        private string ProjectCode => _settings.ProjectCode;

        protected AbstractNLogObserver(string loggerName, IProgramSettings settings)
        {
            _loggerName = loggerName;
            _settings = settings;
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
            eventInfo.Properties["LogDirectoryName"] = LogDirectory;
            eventInfo.Properties["ProjectCode"] = ProjectCode.ToDefaultIfEmpty().ToUpperInvariant();
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
}