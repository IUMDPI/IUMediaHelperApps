using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Common.TaskScheduler.Extensions;
using Common.TaskScheduler.Models;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public abstract class AbstractScheduler : ITaskScheduler
    {
        private string Identifier { get; }
        
        protected AbstractScheduler(string identifier)
        {
            Identifier = identifier;
        }

        public void Remove(string taskname)
        {
            using (var service = new TaskService())
            {
                service.RootFolder.DeleteTask(taskname, false);
            }
        }

        public void Stop(string taskName)
        {
            using (var service = new TaskService())
            {
                var task = service.RootFolder.AllTasks.SingleOrDefault(t => t.Name.Equals(taskName));
                if (task == null)
                {
                    return;
                }

                task.Stop();
            }
        }

        public void Enable(string taskName, bool enable)
        {
            using (var service = new TaskService())
            {
                var task = service.RootFolder.AllTasks.SingleOrDefault(t => t.Name.Equals(taskName));
                if (task == null)
                {
                    return;
                }

                task.Enabled = enable;
            }
        }

        public void ScheduleNonInteractive(string taskName, string targetPath, string arguments, string username, SecureString password, DateTime startOn, DaysOfTheWeek days)
        {
            using (var service = new TaskService())
            {
                var definition = service.NewTask()
                    .ConfigureBaseDetails(targetPath, arguments)
                    .ConfigureWeekly(startOn, days)
                    .AddIdentifier(Identifier)
                    .SetDescription(
                        $"Runs {Path.GetFileName(targetPath)} every {string.Join(",", days)} at {startOn:hh:mm tt}");

                service.RootFolder.RegisterTaskDefinition(taskName, definition,
                    TaskCreation.CreateOrUpdate, username,
                    password.ToUnsecureString(), TaskLogonType.Password);

            }
        }
        
        public void ScheduleInteractive(string taskName, string targetPath, string arguments, DateTime startOn, DaysOfTheWeek days)
        {
            using (var service = new TaskService())
            {
                var definition = service.NewTask()
                    .ConfigureBaseDetails(targetPath, arguments)
                    .ConfigureWeekly(startOn, days)
                    .AddIdentifier(Identifier)
                    .SetDescription(
                        $"Runs {Path.GetFileName(targetPath)} every {string.Join(",", days)} at {startOn:hh:mm tt}");

                service.RootFolder.RegisterTaskDefinition(taskName, definition);
            }
        }

        public Task FindExisting()
        {
            using (var service = new TaskService())
            {
                return service.RootFolder.AllTasks.FirstOrDefault(IsTaskInstance);
            }
        }

        private bool IsTaskInstance(Task task)
        {
            try
            {
                return IsInstanceDefinition(task.Definition) || HasInstanceAction(task.Definition);
            }
            catch
            {
                return false;
            }
        }

        private bool HasInstanceAction(TaskDefinition definition)
        {
            return definition.Actions.Any(action => IsActionInstance(action as ExecAction));
        }

        private bool IsActionInstance(ExecAction action)
        {
            if (action == null)
            {
                return false;
            }

            return IsRecognizedAssembly(action.Path);
        }

        private bool IsInstanceDefinition(TaskDefinition definition)
        {

            return !string.IsNullOrWhiteSpace(definition.Data)
                   && definition.Data.Equals(Identifier);
        }
        
        public void OpenWindowsTaskScheduler()
        {
            Process.Start("taskschd.msc");
        }

        public bool IsRecognizedAssembly(string path)
        {
            var assemblyIdentifier = TryGetIdentifierFromAssembly(path);
            return assemblyIdentifier.ToLowerInvariant().Equals(Identifier);
        }

        private static string TryGetIdentifierFromAssembly(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return string.Empty;
                }

                var assembly = Assembly.LoadFrom(path);
                var attribute = assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0] as GuidAttribute;

                return attribute != null 
                    ? attribute.Value 
                    : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public abstract TaskConfiguration GetDefaultConfiguration();
    }
}
