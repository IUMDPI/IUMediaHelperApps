using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using Scheduler.Models;

namespace Scheduler
{
    internal class Program
    {
        private const DaysOfTheWeek DefaultDaysOfTheWeek =
            DaysOfTheWeek.Monday |
            DaysOfTheWeek.Tuesday |
            DaysOfTheWeek.Wednesday |
            DaysOfTheWeek.Thursday |
            DaysOfTheWeek.Friday;

        private const string DefaultName = "Media Packager";
        private const string DefaultExecutable = "Packager.exe";
        private const string QuietPrefix = "-q";

        private static int Main(string[] args)
        {
            var result = 0;
            
            try
            {
                var defaultStartTime = DateTime.Now.Date.AddHours(19);
                var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), DefaultExecutable);
                var arguments = Arguments.Import(args, DefaultName, defaultPath, DefaultDaysOfTheWeek, defaultStartTime);

                ValidateArgs(arguments);

                using (var service = new TaskService())
                {
                    RemoveExistingTask(service.RootFolder, arguments.Name);

                    var definition = service.NewTask();

                    definition.Settings.Hidden = false;
                    definition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                    definition.Settings.RunOnlyIfIdle = false;
                    definition.Settings.WakeToRun = true;

                    definition.RegistrationInfo.Description = string.Format("Runs {0} every {1} at {2}",
                        Path.GetFileName(arguments.Target),
                        string.Join(",", arguments.Days),
                        string.Format("{0:hh:mm tt}", arguments.StartTime));

                    var trigger = new WeeklyTrigger(arguments.Days)
                    {
                        StartBoundary = arguments.StartTime
                    };
                    definition.Triggers.Add(trigger);

                    definition.Actions.Add(new ExecAction(arguments.Target, null, Path.GetDirectoryName(arguments.Target)));

                    service.RootFolder.RegisterTaskDefinition(arguments.Name, definition);
                }

                WriteSuccessMessage(arguments);

                result = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = -1;
            }

            DoWait(args);
            return result;
        }

        private static void WriteSuccessMessage(Arguments arguments)
        {
            Console.WriteLine("{0} successfully scheduled to run at {1} every {2}. Scheduled task name is '{3}.'",
                    Path.GetFileName(arguments.Target),
                    string.Format("{0:hh:mm tt}", arguments.StartTime),
                    string.Join(",", arguments.Days), arguments.Name);
    }

        private static void DoWait(IEnumerable<string> args)
        {
            if (args.Any(a => a.Equals(QuietPrefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }
        
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static void RemoveExistingTask(TaskFolder folder, string name)
        {
            folder.DeleteTask(name, false);
        }

        private static void ValidateArgs(Arguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.Name))
            {
                throw new Exception("Task name not specified. Use -name:[value] to specify name of scheduled task.");
            }

            if (string.IsNullOrWhiteSpace(arguments.Target))
            {
                throw new Exception("Target executable not specified. Use -target:[value] to specify target executable.");
            }

            if (!File.Exists(arguments.Target))
            {
                throw new Exception(string.Format("Target executable {0} does not exist", arguments.Target));
            }
        }
    }
}