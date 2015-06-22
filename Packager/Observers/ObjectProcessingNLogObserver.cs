using System;
using NLog;
using Packager.Processors;

namespace Packager.Observers
{
    public class ObjectProcessingNLogObserver : AbstractNLogObserver
    {
        private string Barcode { get; set; }
        private string ProjectCode { get; set; }
        
        private const string ThisLoggerName = "ObjectProcessingFileLogger";
 
        public ObjectProcessingNLogObserver(string barcode, string projectCode, string logDirectory, string processingDirectory)
            : base(logDirectory, processingDirectory)
        {
            Barcode = barcode;
            ProjectCode = projectCode;
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

        protected override string LoggerName
        {
            get { return ThisLoggerName; }
        }


        protected override LogEventInfo GetLogEvent(string baseMessage, params object[] elements)
        {
            var logEvent = base.GetLogEvent(baseMessage, elements);
            logEvent.Properties["Barcode"] = Barcode;
            logEvent.Properties["ProjectCode"] = ProjectCode;
            return logEvent;
        }

        private static readonly Logger Logger = LogManager.GetLogger(ThisLoggerName);
    }
}