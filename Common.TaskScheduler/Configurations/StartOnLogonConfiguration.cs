using System;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Configurations
{
    public class StartOnLogonConfiguration:AbstractConfiguration
    {
        public string Username { get; set; }
        public TimeSpan Delay { get; set; }

        public StartOnLogonConfiguration()
        {
        }

        private StartOnLogonConfiguration(Task task) : base(task)
        {
            var trigger = task.Definition.GetLogonTrigger();
            if (trigger == null)
            {
                return;
            }

            Username = trigger.UserId;
            Delay = trigger.Delay;
        }

        public override AbstractConfiguration Import(Task task)
        {
            var trigger = task.Definition.GetLogonTrigger();
            return trigger != null 
                ? new StartOnLogonConfiguration(task)
                : null;
        }

        protected override TaskDefinition ConfigureDefinition(TaskDefinition definition)
        {
            return base.ConfigureDefinition(definition)
                .ConfigureForUserLogon(Username, Delay);
        }
    }
}
