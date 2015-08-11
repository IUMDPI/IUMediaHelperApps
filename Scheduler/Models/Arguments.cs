using System;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace Scheduler.Models
{
    public class Arguments
    {
        private const string TargetPrefix = "-target:";
        private const string DaysPrefix = "-days:";
        private const string StartPrefix = "-start:";
        private const string NamePrefix = "-name:";
        public string Target { get; set; }
        public DaysOfTheWeek Days { get; set; }
        public DateTime StartTime { get; set; }
        public string Name { get; set; }

        public static Arguments Import(string[] args, string defaultName, string defaultTarget, DaysOfTheWeek defaultDays, DateTime defaultStartTime)
        {
            var arguments = new Arguments
            {
                Name = args.SingleOrDefault(a => a.ToLowerInvariant().StartsWith(NamePrefix)),
                Target = args.SingleOrDefault(a => a.ToLowerInvariant().StartsWith(TargetPrefix)),
                Days = ResolveDays(args.SingleOrDefault(a => a.ToLowerInvariant().StartsWith(DaysPrefix)), defaultDays),
                StartTime = ResolveStartTime(args.SingleOrDefault(a => a.ToLowerInvariant().StartsWith(StartPrefix)), defaultStartTime)
            };
            
            if (string.IsNullOrWhiteSpace(arguments.Target))
            {
                arguments.Target = defaultTarget;
            }

            if (arguments.Days == 0)
            {
                arguments.Days = defaultDays;
            }
            
            if (string.IsNullOrWhiteSpace(arguments.Name))
            {
                arguments.Name = defaultName;
            }

            return arguments;
        }

        private static DateTime ResolveStartTime(string startTime, DateTime defaultStartTime)
        {
            if (string.IsNullOrWhiteSpace(startTime))
            {
                return defaultStartTime;
            }

            TimeSpan timespan;
            if (!TimeSpan.TryParse(startTime, out timespan))
            {
                throw new Exception(string.Format("Could not convert {0} to valid start time. Valid format is 'hh:mm:ss'", startTime));
            }

            return DateTime.Now.Date.Add(timespan);
        }

        private static DaysOfTheWeek ResolveDays(string daysValue, DaysOfTheWeek defaultDays)
        {
            if (string.IsNullOrWhiteSpace(daysValue))
            {
                return defaultDays;
            }

            var parts = daysValue.Split(',');
            DaysOfTheWeek result = 0;

            foreach (var part in parts)
            {
                DaysOfTheWeek value;
                if (!Enum.TryParse(part.Trim(), true, out value))
                {
                    throw new Exception(string.Format("Could not convert {0} to valid day value. Allowed values are 'Monday', 'Tuesday','Wednesday', 'Thursday', 'Friday', 'Saturday',  and 'Sunday'",
                        part));
                }

                result = (result | value);
            }

            return result;
        }
    }
}