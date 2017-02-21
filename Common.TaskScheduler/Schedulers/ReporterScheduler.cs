using System;
using Common.TaskScheduler.Models;

namespace Common.TaskScheduler.Schedulers
{
    public class ReporterScheduler:AbstractScheduler
    {
        public ReporterScheduler() : base(Constants.ReporterGuid)
        {
        }

        public override TaskConfiguration GetDefaultConfiguration()
        {
            return new TaskConfiguration
            {
                TaskName = "Media Reporter",
                RunNonInteractive = false,
                RunOnUserLogon = true,
                StartTime = DateTime.Now.Date.AddHours(19),
            };
        }
    }
}
