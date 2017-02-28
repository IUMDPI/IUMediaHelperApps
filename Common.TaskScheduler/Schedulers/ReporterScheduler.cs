using System;
using System.Security.Principal;
using Common.TaskScheduler.Configurations;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public class ReporterScheduler:AbstractScheduler
    {
        public ReporterScheduler() : base(Constants.ReporterGuid)
        {
        }

        public override AbstractConfiguration GetDefaultConfiguration()
        {
            return new StartOnLogonConfiguration
            {
                Username = WindowsIdentity.GetCurrent().Name,
                Delay = new TimeSpan(0,2,0)
            };
        }

        protected override AbstractConfiguration Import(Task task)
        {
            return new StartOnLogonConfiguration().Import(task);
        }
    }
}
