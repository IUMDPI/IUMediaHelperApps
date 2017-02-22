using System;
using Common.TaskScheduler.Configurations;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public class PackagerScheduler : AbstractScheduler
    {
        public PackagerScheduler() : base(Constants.PackagerGuid)
        {
        }

        public override AbstractConfiguration GetDefaultConfiguration()
        {
            return new NonInteractiveDailyConfiguration
            {
                TaskName = "Media Packager",
                StartOn = DateTime.Now.Date.AddHours(19),
                Days = DaysOfTheWeek.Monday | 
                            DaysOfTheWeek.Tuesday |
                            DaysOfTheWeek.Wednesday |
                            DaysOfTheWeek.Thursday |
                            DaysOfTheWeek.Friday,
                
            };
        }
    }
}
