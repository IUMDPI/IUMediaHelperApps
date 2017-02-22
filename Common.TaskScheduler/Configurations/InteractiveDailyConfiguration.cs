using System;
using System.Collections.Generic;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Configurations
{
    public class InteractiveDailyConfiguration:AbstractConfiguration
    {
        public DateTime StartOn { get; set; }
        public DaysOfTheWeek Days { get; set; }

        protected override List<string> Verify()
        {
            var issues = base.Verify();
            if (Days == 0)
            {
                issues.Add("Please select at least one day.");
            }

            return issues;
        }

        protected override TaskDefinition ConfigureDefinition(TaskDefinition definition)
        {
            return base.ConfigureDefinition(definition)
                .ConfigureWeekly(StartOn, Days);
        }
    }
}
