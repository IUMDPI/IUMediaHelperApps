using System;
using Common.TaskScheduler.Models;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public class PackagerScheduler : AbstractScheduler
    {
        public PackagerScheduler() : base(Constants.PackagerGuid)
        {
        }

        public override TaskConfiguration GetDefaultConfiguration()
        {
            return new TaskConfiguration
            {
                TaskName = "Media Packager",
                StartTime = DateTime.Now.Date.AddHours(19),
                RunNonInteractive = true,
                RunOnDays = DaysOfTheWeek.Monday | 
                            DaysOfTheWeek.Tuesday |
                            DaysOfTheWeek.Wednesday |
                            DaysOfTheWeek.Thursday |
                            DaysOfTheWeek.Friday,
                RunOnUserLogon = false
            };
        }
    }
}
