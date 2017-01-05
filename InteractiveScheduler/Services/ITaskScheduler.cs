using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using InteractiveScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace InteractiveScheduler.Services
{
    public interface ITaskScheduler
    {
        void Schedule(string taskname, string targetPath, string arguments, string username, SecureString password, DateTime startOn, DaysOfTheWeek days);

        Task FindExisting();
        void Remove(string taskName);
        void Stop(string taskName);
        void Open(string taskName);
        void Enable(string taskName, bool enable);
    }

    public class TaskScheduler : ITaskScheduler
    {
        private const string DataText = "Media Packager";

        public void Schedule(string taskName, string targetPath, string arguments, string username, SecureString password, DateTime startOn, DaysOfTheWeek days)
        {

            using (var service = new TaskService())
            {
                var definition = service.NewTask();
                definition.Settings.Hidden = false;
                definition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                definition.Settings.RunOnlyIfIdle = false;
                definition.Settings.WakeToRun = true;
                definition.Settings.RunOnlyIfNetworkAvailable = true;
                definition.Settings.Compatibility = TaskCompatibility.V2;
                definition.RegistrationInfo.Author = "Indiana University";
                definition.RegistrationInfo.Version = new Version(1,0,0);
                definition.RegistrationInfo.Description =
                    $"Runs {Path.GetFileName(targetPath)} every {string.Join(",", days)} at {startOn:hh:mm tt}";

                var trigger = new WeeklyTrigger(days)
                {
                    StartBoundary = startOn
                };
                definition.Triggers.Add(trigger);
                definition.Data = DataText;
                definition.Actions.Add(new ExecAction(targetPath, arguments, Path.GetDirectoryName(targetPath)));

                if (IsLoginRequired(password))
                {
                    service.RootFolder.RegisterTaskDefinition(taskName, definition, TaskCreation.CreateOrUpdate, username,
                        password.ToUnsecureString(), TaskLogonType.Password);
                }
                else
                {
                    service.RootFolder.RegisterTaskDefinition(taskName, definition);
                }
            }
        }

        private static bool IsLoginRequired(SecureString password)
        {
            return password != null && password.Length > 0;
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

        public void Open(string taskName)
        {
            Process.Start("taskschd.msc");
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

                task.Enabled =enable;
            }
        }

        public Task FindExisting()
        {
            using (var service = new TaskService())
            {
                return service.RootFolder.AllTasks.FirstOrDefault(IsPackagerTask);
            }
        }

        private static bool IsPackagerTask(Task task)
        {
            return IsPackagerDefinition(task.Definition) || HasPackagerAction(task.Definition);
        }

        private static bool HasPackagerAction(TaskDefinition definition)
        {
            return definition.Actions.Any(action => IsPackagerAction(action as ExecAction));
        }

        private static bool IsPackagerAction(ExecAction action)
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
            return info.ProductName.ToLowerInvariant().Equals("packager");
        }

        private static bool IsPackagerDefinition(TaskDefinition definition)
        {
            return !string.IsNullOrWhiteSpace(definition.Data) 
                && definition.Data.Equals(DataText);
        }
    }
}


