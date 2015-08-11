using System;
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

        private static int Main(string[] args)
        {
            var defaultStartTime = DateTime.Now.Date.AddHours(19);
            
            var arguments = Arguments.Import(args, "Media Packager", Path.Combine(Directory.GetCurrentDirectory(), "Packager.exe"),
                DefaultDaysOfTheWeek, defaultStartTime);

            if (!ValidateArgs(arguments))
            {
                return -1;
            }

            using (var service = new TaskService())
            {
                
                RemoveExistingTask(service.RootFolder, arguments.Name);

                var definition = service.NewTask();
                
                definition.RegistrationInfo.Description = string.Format("Runs {0} every {1} at {2}",
                    Path.GetFileName(arguments.Target),
                    string.Join(",", arguments.Days),
                    arguments.StartTime.TimeOfDay);

                var trigger = new WeeklyTrigger(arguments.Days)
                {
                    StartBoundary = arguments.StartTime
                };
                definition.Triggers.Add(trigger);

                definition.Actions.Add(new ExecAction(arguments.Target, null, Path.GetDirectoryName(arguments.Target)));
                
                service.RootFolder.RegisterTaskDefinition(arguments.Name, definition);
            }

            return 0;
        }

        private static void RemoveExistingTask(TaskFolder folder, string name)
        {
            folder.DeleteTask(name, false);
        }
        
        
        private static bool ValidateArgs(Arguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.Name))
            {
                Console.WriteLine("Task name not specified. Use -name:[value] to specify name of scheduled task.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(arguments.Target))
            {
                Console.WriteLine("Target executable not specified. Use -target:[value] to specify target executable.");
                return false;
            }

            /*if (!File.Exists(arguments.Target))
            {
                Console.WriteLine("Target executable {0} does not exist", arguments.Target);
                return false;
            }*/
            
            return true;
        }
    }
}