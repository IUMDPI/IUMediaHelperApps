using System;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;

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
                var target = GetFileTarget(LogManager.Configuration.FindTargetByName("GeneralFileLogger"));
                if (target == null)
                {
                    return "";
                }

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                logEventInfo.Properties.Add("LogDirectoryName", LogFolder);
                return target.FileName.Render(logEventInfo);
            }
        }

        private static FileTarget GetFileTarget(Target value)
        {
            var wrapper = value as AsyncTargetWrapper;
            var actual = (wrapper != null)
                ? wrapper.WrappedTarget
                : value;


            return actual as FileTarget;
        }
    }
}