using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Configurations
{
    public abstract class AbstractConfiguration
    {
        public string TaskName { get; set; }
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
        public bool Running { get; }
        public bool Enabled { get; }
        public bool Exists { get; }
        
        public abstract AbstractConfiguration Import(Task task);
        
        protected AbstractConfiguration(Task task)
        {
            TaskName = task.Name;
            ExecutablePath = task.Definition.GetExecutablePath();
            Arguments = task.Definition.GetExecutableArguments();
            Running = task.State == TaskState.Running;
            Enabled = task.Enabled;
            Exists = true;
        }

        protected AbstractConfiguration()
        {
            TaskName = string.Empty;
            ExecutablePath = string.Empty;
            Running = false;
            Enabled = false;
            Exists = false;
        }

        protected virtual List<string> Verify()
        {
            var issues = new List<string>();
            if (string.IsNullOrWhiteSpace(TaskName))
            {
                issues.Add("Please specify a valid task name");
            }

            if (!File.Exists(ExecutablePath))
            {
                issues.Add("Please provide a valid executable.");
            }

            return issues;
        }

        protected virtual TaskDefinition ConfigureDefinition(TaskDefinition definition)
        {
            return definition.ConfigureBaseDetails(ExecutablePath, Arguments);
        }

        public Tuple<bool, List<string>> Schedule(string identifier)
        {
            var issues = Verify();
            if (issues.Any())
            {
                return new Tuple<bool, List<string>>(false, issues);
            }

            try
            {
                using (var service = new TaskService())
                {
                    var definition = ConfigureDefinition(service.NewTask())
                        .AddIdentifier(identifier);

                    return ScheduleInternal(service, definition);
                }
            }
            catch (Exception e)
            {
                return new Tuple<bool, List<string>>(
                    false,
                    new List<string> { $"An exception occurred while scheduling the task: {e.Message}" });
            }
            
        }

        protected virtual Tuple<bool, List<string>> ScheduleInternal(TaskService service, TaskDefinition definition)
        {
            service.RootFolder.RegisterTaskDefinition(TaskName, definition);
            return new Tuple<bool, List<string>>(true, new List<string>());
        }
    }
}
