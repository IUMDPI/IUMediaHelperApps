using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace Scheduler.Models
{
    public class Arguments
    {
        private const string TargetPrefix = "-target=";
        private const string DaysPrefix = "-days=";
        private const string StartPrefix = "-start=";
        private const string NamePrefix = "-name=";
        
        public string Target { get; set; }
        public DaysOfTheWeek Days { get; set; }
        public DateTime StartTime { get; set; }
        public string Name { get; set; }
        
      

        public static Arguments Import(string[] args, string defaultName, string defaultTarget, DaysOfTheWeek defaultDays, DateTime defaultStartTime)
        {
            return new Arguments
            {
                Name = GetValueForToken(args, NamePrefix, defaultName),
                Target = GetValueForToken(args, TargetPrefix, defaultTarget),
                Days = ResolveDays(args, defaultDays),
                StartTime = ResolveStartTime(args, defaultStartTime),
            };
        }

        private static string GetValueForToken(IEnumerable<string> args, string token, string defaultValue="")
        {
            var result = args.SingleOrDefault(a => a.ToLowerInvariant().StartsWith(token));
            return string.IsNullOrWhiteSpace(result) 
                ? defaultValue 
                : result.Remove(0, token.Length);
        }

        private static DateTime ResolveStartTime(IEnumerable<string> args, DateTime defaultStartTime)
        {
            var startTime = GetValueForToken(args, StartPrefix);

            if (string.IsNullOrWhiteSpace(startTime))
            {
                return defaultStartTime;
            }

            TimeSpan timespan;
            if (!TimeSpan.TryParse(startTime, out timespan))
            {
                throw new Exception($"Could not convert {startTime} to valid start time. Valid format is hh:mm:ss.");
            }

            return DateTime.Now.Date.Add(timespan);
        }

        private static DaysOfTheWeek ResolveDays(IEnumerable<string> args, DaysOfTheWeek defaultDays)
        {
            var daysValue = GetValueForToken(args, DaysPrefix);
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
                    throw new Exception($"Could not convert {part} to valid day value. Allowed values are 'Monday', 'Tuesday','Wednesday', 'Thursday', 'Friday', 'Saturday',  and 'Sunday.'");
                }

                result = (result | value);
            }

            return result > 0 ? result:defaultDays;
        }
    }
}