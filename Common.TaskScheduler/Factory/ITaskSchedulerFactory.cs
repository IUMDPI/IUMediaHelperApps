using Common.TaskScheduler.Schedulers;

namespace Common.TaskScheduler.Factory
{
    public interface ITaskSchedulerFactory
    {
        ITaskScheduler GetForApplication(string path);
        ITaskScheduler GetDefaultScheduler();
    }
}
