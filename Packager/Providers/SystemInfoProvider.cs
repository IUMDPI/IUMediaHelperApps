using System;
using System.Windows;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Packager.Engine;
using Packager.Models.SettingsModels;

namespace Packager.Providers
{
    public class SystemInfoProvider : ISystemInfoProvider
    {
        public SystemInfoProvider(IProgramSettings programSettings)
        {
            LogFolder = programSettings.LogDirectoryName;
        }

        private string LogFolder { get; }
        
        public string MachineName => Environment.MachineName;

        public string CurrentSystemLogPath
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

        public void ExitApplication(EngineExitCodes exitCode)
        {
            Application.Current.Shutdown((int) exitCode);
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