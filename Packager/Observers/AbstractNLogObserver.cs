using System;
using NLog;
using Packager.Exceptions;

namespace Packager.Observers
{
    public abstract class AbstractNLogObserver:IObserver
    {
        public abstract void Log(string baseMessage, params object[] elements);
        public abstract void LogProcessingError(Exception issue, string barcode);
        
        protected abstract Logger Logger { get; }

        public void LogEngineError(Exception issue)
        {
            if (issue is LoggedException)
            {
                return;
            }

            Log(issue.ToString());
        }
        
        protected abstract string LoggerName { get;}

        protected AbstractNLogObserver(string logDirectory, string processingDirectory)
        {
            LogDirectory = logDirectory;
            ProcessingDirectory = processingDirectory;
        }

        protected string LogDirectory { get; private set; }
        protected string ProcessingDirectory { get; private set; }

        protected void Log(LogEventInfo logEvent)
        {
            if (string.IsNullOrWhiteSpace(logEvent.Message))
            {
                return;
            }

            Logger.Log(logEvent);
        }

        protected virtual LogEventInfo GetLogEvent(string baseMessage, params object[] elements)
        {
            var eventInfo = LogEventInfo.Create(LogLevel.Info, LoggerName, string.Format(baseMessage, elements));
            eventInfo.Properties["LogDirectoryName"] = LogDirectory;
            eventInfo.Properties["ProcessingDirectoryName"] = ProcessingDirectory;
            
            return eventInfo;
        }

      
    }
}
