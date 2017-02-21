using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Extensions
{
    public static class TaskDefinitionExtensions
    {
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
