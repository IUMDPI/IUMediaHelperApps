using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Configurations
{
    public class NonInteractiveDailyConfiguration : InteractiveDailyConfiguration
    {
        public string Username { get; set; }
        public SecureString Passphrase { get; set; }

        protected override List<string> Verify()
        {
            var issues = base.Verify();
            if (string.IsNullOrWhiteSpace(Username))
            {
                issues.Add("Please provide a username or uncheck Impersonate.");
            }

            if (Passphrase == null || Passphrase.Length == 0)
            {
                issues.Add("Please provide a password or uncheck Impersonate.");
            }

            if (ValidateCredentials(Username, Passphrase) == false)
            {
                issues.Add("Invalid username and/or password.");
            }

            return issues;
        }

        private static bool ValidateCredentials(string username, SecureString password)
        {
            if (password == null || password.Length == 0)
            {
                return false;
            }

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                return context.ValidateCredentials(username, password.ToUnsecureString());
            }
        }

        protected override Tuple<bool, List<string>> ScheduleInternal(TaskService service, TaskDefinition definition)
        {
            service.RootFolder.RegisterTaskDefinition(TaskName, definition,
                    TaskCreation.CreateOrUpdate, Username,
                    Passphrase.ToUnsecureString(), TaskLogonType.Password);
            return new Tuple<bool, List<string>>(true, new List<string>());
        }
    }
}