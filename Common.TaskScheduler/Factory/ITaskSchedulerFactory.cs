using System.Collections.Generic;
using Common.TaskScheduler.Configurations;
using Common.TaskScheduler.Schedulers;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Factory
{
    public interface ITaskSchedulerFactory
    {
        ITaskScheduler GetForApplication(string path);
        ITaskScheduler GetDefaultScheduler();
        List<AbstractConfiguration> GetExistingTasks();
    }
}
