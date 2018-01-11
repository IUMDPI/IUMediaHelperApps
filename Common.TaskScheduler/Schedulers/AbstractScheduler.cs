using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Common.TaskScheduler.Configurations;
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

        public IEnumerable<AbstractConfiguration> GetExistingConfigurations()
        {
            return GetExistingScheduledTasks().Select(Import);
        }

        public IEnumerable<Task> GetExistingScheduledTasks()
        {
            using (var service = new TaskService())
            {
                return service.RootFolder.Tasks
                    .Where(IsTaskInstance);
            }
        }

        protected abstract AbstractConfiguration Import(Task task);

        private bool IsTaskInstance(Task task)
        {
            try
            {
                return task.Definition != null
                    && (IsInstanceDefinition(task.Definition) || HasInstanceAction(task.Definition));
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
            return action != null && IsRecognizedAssembly(action.Path);
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
                var attribute = assembly.GetCustomAttribute<GuidAttribute>();

                return attribute != null
                    ? attribute.Value
                    : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public abstract AbstractConfiguration GetDefaultConfiguration();

        public virtual Tuple<bool, List<string>> Schedule<T>(T configuration) where T : AbstractConfiguration
        {
            return configuration.Schedule(Identifier);
        }

       
    }
}
