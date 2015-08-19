using System;
using NLog;
using NLog.Targets;

namespace Packager.Providers
{
    public class SystemInfoProvider : ISystemInfoProvider
    {
        public SystemInfoProvider(string logFolder)
        {
            LogFolder = logFolder;
        }

        private string LogFolder { get; set; }
        
        public string MachineName => Environment.MachineName;

        public string CurrentLogPath
        {
            get
            {
                var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("GeneralFileLogger");
                if (fileTarget == null)
                {
                    return "";
                }

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                logEventInfo.Properties.Add("LogDirectoryName", LogFolder);
                return fileTarget.FileName.Render(logEventInfo);
            }
        }
    }
}