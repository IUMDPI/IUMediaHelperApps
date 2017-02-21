using System;
using System.Security;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedule the task to logon as a specific user and run non-interactively
        /// </summary>
        /// <param name="targetPath">The path to the program to run</param>
        /// <param name="arguments">The arguments to pass to the program</param>
        /// <param name="username">The username to use when running the task</param>
        /// <param name="password">The password to use when running the task</param>
        /// <param name="startOn">The time to start the task</param>
        /// <param name="days">The days of the week when the task should be run</param>
        void ScheduleNonInteractive(string targetPath, string arguments, string username, SecureString password, DateTime startOn, DaysOfTheWeek days);

        /// <summary>
        /// Schedule the task to run interactively when the user is logged in
        /// </summary>
        /// <param name="targetPath">The path to the program to run</param>
        /// <param name="arguments">The arguments to pass to the program</param>
        /// <param name="startOn">The time to start the task</param>
        /// <param name="days">The days of the week when the task should be run</param>
        void ScheduleInteractive(string targetPath, string arguments, DateTime startOn, DaysOfTheWeek days);

        /// <summary>
        /// Find an existing task scheduler instance.
        /// </summary>
        /// <returns></returns>
        Task FindExisting();

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
    }
}
