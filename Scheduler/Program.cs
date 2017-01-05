using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private const string HelpPrefix = "-?";

        private static int Main(string[] args)
        {
            var result = 0;

            try
            {
                if (ShowHelp(args))
                {
                    DoWait(args);
                    return result;
                }

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
                    definition.RegistrationInfo.Description = $"Runs {Path.GetFileName(arguments.Target)} every {string.Join(",", arguments.Days)} at {arguments.StartTime:hh:mm tt}";

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

        private static bool ShowHelp(string[] args)
        {
            if (args.Any() && !args.Any(a => a.Equals(HelpPrefix)))
            {
                return false;
            }

            Console.WriteLine("Task schedule helper ({0})", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Parameters:");
            Console.WriteLine("");
            Console.WriteLine("  -?            Show this screen");
            Console.WriteLine("  -q            Do not prompt to exit");
            Console.WriteLine("  -target=      Path to executable to schedule.");
            Console.WriteLine("                If not set, will default to packager.exe");
            Console.WriteLine("                in scheduler's working directory.");
            Console.WriteLine("");
            Console.WriteLine("  -days=        Comma-delimited list of days on which to run");
            Console.WriteLine("                the target executable. Valid values are");
            Console.WriteLine("                Monday, Tuesday, Wednesday, Thursday, Friday,");
            Console.WriteLine("                Saturday, and Sunday. If not set, defaults to");
            Console.WriteLine("                'Monday,Tuesday,Wednesday,Thursday,Friday.'");
            Console.WriteLine("");
            Console.WriteLine("                Do not seperate values with spaces.");
            Console.WriteLine("");
            Console.WriteLine("  -start=       The time (in 24-hour format) to start the target");
            Console.WriteLine("                executable on each scheduled day. If not set,");
            Console.WriteLine("                defaults to '19:00' (7 PM)");
            Console.WriteLine("");
            Console.WriteLine("  -name=        The name to use when creating the scheduled task.");
            Console.WriteLine("                If not set, defaults to 'Media Packager.'");

            return true;
        }

        private static void WriteSuccessMessage(Arguments arguments)
        {
            Console.WriteLine("{0} successfully scheduled to run at {1} every {2}. Scheduled task name is '{3}.'",
                Path.GetFileName(arguments.Target),
                $"{arguments.StartTime:hh:mm tt}",
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
                throw new Exception($"Target executable {arguments.Target} does not exist");
            }
        }
    }
}