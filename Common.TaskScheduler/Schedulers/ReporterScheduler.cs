using System;
using Common.TaskScheduler.Configurations;

namespace Common.TaskScheduler.Schedulers
{
    public class ReporterScheduler:AbstractScheduler
    {
        public ReporterScheduler() : base(Constants.ReporterGuid)
        {
        }

        public override AbstractConfiguration GetDefaultConfiguration()
        {
            return new StartOnLogonConfiguration
            {
                TaskName = "Media Reporter"
            };
        }
    }
}
