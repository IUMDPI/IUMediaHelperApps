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
            return new ImpersonateDailyConfiguration
            {
                StartOn = DateTime.Now.Date.AddHours(19),
                Days = DaysOfTheWeek.Monday | 
                            DaysOfTheWeek.Tuesday |
                            DaysOfTheWeek.Wednesday |
                            DaysOfTheWeek.Thursday |
                            DaysOfTheWeek.Friday,
                
            };
        }

        protected override AbstractConfiguration Import(Task task)
        {
            return new ImpersonateDailyConfiguration().Import(task) 
                ?? new DailyConfiguration().Import(task);
        }

       
    }
}
