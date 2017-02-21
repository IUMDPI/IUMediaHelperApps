using System.Collections.Generic;
using System.Linq;
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
                new PackagerScheduler()
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
    }
}