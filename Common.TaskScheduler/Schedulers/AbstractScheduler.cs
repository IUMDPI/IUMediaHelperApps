using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public abstract class AbstractScheduler : ITaskScheduler
    {
        private string TaskName { get; }
        private string Identifier { get; }
        private string ProductName { get; }

        protected AbstractScheduler(string taskName, string identifier, string productName)
        {
            TaskName = taskName;
            Identifier = identifier;
            ProductName = productName;
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

        public void ScheduleNonInteractive(string targetPath, string arguments, string username, SecureString password, DateTime startOn, DaysOfTheWeek days)
        {
            using (var service = new TaskService())
            {
                var definition = service.NewTask()
                    .ConfigureBaseDetails(targetPath, arguments)
                    .ConfigureWeekly(startOn, days)
                    .AddIdentifier(Identifier)
                    .SetDescription(
                        $"Runs {Path.GetFileName(targetPath)} every {string.Join(",", days)} at {startOn:hh:mm tt}");

                service.RootFolder.RegisterTaskDefinition(TaskName, definition,
                    TaskCreation.CreateOrUpdate, username,
                    password.ToUnsecureString(), TaskLogonType.Password);

            }
        }
        
        public void ScheduleInteractive(string targetPath, string arguments, DateTime startOn, DaysOfTheWeek days)
        {
            using (var service = new TaskService())
            {
                var definition = service.NewTask()
                    .ConfigureBaseDetails(targetPath, arguments)
                    .ConfigureWeekly(startOn, days)
                    .AddIdentifier(Identifier)
                    .SetDescription(
                        $"Runs {Path.GetFileName(targetPath)} every {string.Join(",", days)} at {startOn:hh:mm tt}");

                service.RootFolder.RegisterTaskDefinition(TaskName, definition);
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
            return IsInstanceDefinition(task.Definition) || HasInstanceAction(task.Definition);
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

            if (!File.Exists(action.Path))
            {
                return false;
            }

            var info = FileVersionInfo.GetVersionInfo(action.Path);
            return info.ProductName.ToLowerInvariant().Equals(ProductName);
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
    }
}
