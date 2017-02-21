using System;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Models
{
    public class TaskConfiguration
    {
        public string TaskName { get; set; }
        public bool RunNonInteractive { get; set; }
        public bool RunOnUserLogon { get; set; }
        public DateTime StartTime { get; set; } 
        public DaysOfTheWeek RunOnDays { get; set; }
    }
}
