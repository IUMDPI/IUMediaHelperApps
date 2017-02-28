using System;
using System.Collections.Generic;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Configurations
{
    public class DailyConfiguration:AbstractConfiguration
    {
        protected DailyConfiguration(Task task) : base(task)
        {
            var trigger = task.Definition.GetWeeeklyTrigger();
            if (trigger != null)
            {
                Days = trigger.DaysOfWeek;
                StartOn = trigger.StartBoundary;
            }
        }

        public DailyConfiguration(){}

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

        public override AbstractConfiguration Import(Task task)
        {
            var trigger = task.Definition.GetWeeeklyTrigger();
            return trigger != null 
                ? new DailyConfiguration(task)
                : null;
        }
    }
}
