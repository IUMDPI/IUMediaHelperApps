using System;
using System.IO;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Extensions
{
    public static class TaskDefinitionExtensions
    {
        public static string GetExecutablePath(this TaskDefinition definition)
        {
            return definition.GetExecutableAction()?.Path;
        }

        public static string GetExecutableArguments(this TaskDefinition definition)
        {
            return definition.GetExecutableAction()?.Arguments;
        }

        private static ExecAction GetExecutableAction(this TaskDefinition definition)
        {
            return definition?.Actions.FirstOrDefault(a => a is ExecAction) as ExecAction;
        }

        public static WeeklyTrigger GetWeeeklyTrigger(this TaskDefinition definition)
        {
            return definition?.Triggers.FirstOrDefault(t=> t is WeeklyTrigger) as WeeklyTrigger;
        }

        public static LogonTrigger GetLogonTrigger(this TaskDefinition definition)
        {
            return definition?.Triggers.FirstOrDefault(t => t is LogonTrigger) as LogonTrigger;
        }

        public static TaskDefinition ConfigureBaseDetails(this TaskDefinition definition, string targetPath, string arguments)
        {
            definition.Settings.Hidden = false;
            definition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
            definition.Settings.RunOnlyIfIdle = false;
            definition.Settings.WakeToRun = true;
            definition.Settings.RunOnlyIfNetworkAvailable = true;
            definition.Settings.Compatibility = TaskCompatibility.V2;
            definition.RegistrationInfo.Version = new Version(1, 0, 0);
            definition.Actions.Add(new ExecAction(targetPath, arguments, Path.GetDirectoryName(targetPath)));
            return definition;
        }

        public static TaskDefinition ConfigureWeekly(this TaskDefinition definition, DateTime startOn,
            DaysOfTheWeek days)
        {
            var trigger = new WeeklyTrigger(days)
            {
                StartBoundary = startOn
            };
            definition.Triggers.Add(trigger);
            
            return definition;
        }

        public static TaskDefinition ConfigureForUserLogon(this TaskDefinition definition, string username, TimeSpan delay)
        {
            var trigger = new LogonTrigger {UserId = username, Delay = delay};
            definition.Triggers.Add(trigger);
            return definition;
        }

        public static TaskDefinition AddIdentifier(this TaskDefinition definition, string identifier)
        {
            definition.Data = identifier;
            return definition;
        }

        public static  TaskDefinition SetDescription(this TaskDefinition definition, string text)
        {
            definition.RegistrationInfo.Description = text;
            return definition;
        }
    }
}
