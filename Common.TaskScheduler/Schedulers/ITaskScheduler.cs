using System;
using System.Collections.Generic;
using Common.TaskScheduler.Configurations;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedule the task
        /// </summary>
        /// <param name="configuration"></param>
        Tuple<bool, List<string>> Schedule<T> (T configuration) where T:
        AbstractConfiguration;

        /// <summary>
        /// Returns a list of existing configurations that are compatible with this scheduler
        /// </summary>
        /// <returns></returns>
        IEnumerable<AbstractConfiguration> GetExistingConfigurations();

        /// <summary>
        /// Returns a list of existing scheduled tasks that are compatible with this scheduler
        /// </summary>
        /// <returns></returns>
        IEnumerable<Task> GetExistingScheduledTasks();

        /// <summary>
        /// Remove an existing task scheduler instance.
        /// </summary>
        /// <param name="taskName">The name of the task to remove</param>
        void Remove(string taskName);

        /// <summary>
        /// Stop an existing task if it is current running
        /// </summary>
        /// <param name="taskName">The name of the task to stop</param>
        void Stop(string taskName);

        /// <summary>
        /// Open the Windows Task Scheduler
        /// </summary>
        void OpenWindowsTaskScheduler();

        /// <summary>
        /// Enable (or disable) an existing task
        /// </summary>
        /// <param name="taskName">The name of the task to enable or disable</param>
        /// <param name="enable">Set to true to enable the task, false to disable</param>
        void Enable(string taskName, bool enable);

        bool IsRecognizedAssembly(string path);

        AbstractConfiguration GetDefaultConfiguration();
    }
}
