using System.Collections.Generic;
using System.Linq;
using Common.TaskScheduler.Configurations;
using Common.TaskScheduler.Schedulers;

namespace Common.TaskScheduler.Factory
{
    public class TaskSchedulerFactory : ITaskSchedulerFactory
    {
        private List<ITaskScheduler> Schedulers { get; }

        public TaskSchedulerFactory()
        {
            Schedulers = new List<ITaskScheduler>
            {
                new PackagerScheduler(),
                new ReporterScheduler()
            };
        }

        public ITaskScheduler GetForApplication(string path)
        {
            return Schedulers.SingleOrDefault(s => s.IsRecognizedAssembly(path));
        }

        public ITaskScheduler GetDefaultScheduler()
        {
            return Schedulers.First();
        }

        public List<AbstractConfiguration> GetExistingTasks()
        {
            return Schedulers.SelectMany(s => s.FindExisting()).ToList();
        }
    }
}