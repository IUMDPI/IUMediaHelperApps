using System;
using NLog;

namespace Packager.Observers
{
    public abstract class AbstractNLogObserver:IObserver
    {
        
        public abstract void Log(string baseMessage, params object[] elements);
        public abstract void LogHeader(string baseMessage, params object[] elements);
        public abstract void LogError(string baseMessage, object[] elements);
        
        public void LogExternal(string text)
        {
            Log(text);
        }

        protected abstract string LoggerName { get;}

        protected AbstractNLogObserver(string logDirectory, string processingDirectory)
        {
            LogDirectory = logDirectory;
            ProcessingDirectory = processingDirectory;
        }

        protected string LogDirectory { get; private set; }
        protected string ProcessingDirectory { get; private set; }
        
        protected virtual LogEventInfo GetLogEvent(string baseMessage, params object[] elements)
        {
            var eventInfo = LogEventInfo.Create(LogLevel.Info, LoggerName, string.Format(baseMessage, elements));
            eventInfo.Properties["LogDirectoryName"] = LogDirectory;
            eventInfo.Properties["ProcessingDirectoryName"] = ProcessingDirectory;
            
            return eventInfo;
        }

      
    }
}
